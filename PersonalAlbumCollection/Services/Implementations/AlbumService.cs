// AlbumService.cs

using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

/// <summary>
/// Serwis zawierający metody do pracy na albumach.
/// </summary>
public class AlbumService : IAlbumService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="context">Zawiera AppDbContext do odczytu.</param>
    /// <param name="currentUser">Zawiera ICurrentUserService do odczytu.</param>
    public AlbumService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    
    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera z bazy dane wszystkich albumów należących do zalogowanego użytkownika.
    /// </summary>
    /// <param name="search">Wyszukiwanie po nazwie lub artyście</param>
    /// <param name="genreId">Wyszukiwanie po gatunku muzycznym</param>
    /// <returns> Lista obiektów /<AlbumListItemDto/> </returns>
    public async Task<List<AlbumListItemDto>> GetAllAsync(string? search = null, int? genreId = null)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Filtruje Artystów i Gatunki należące do danego użytkownika.
        var query = _context.Albums
            .AsNoTracking()  // Używane kiedy nie modyfikujemy pobranych encji, co poprawia wydajność.
            .Where(a => a.UserId == userId)  
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
            .AsQueryable();

        // Gdy search != null, normalizuje do małych liter i filtruje albumy po frazie w nazwie lub artyście.
        // Dodaje warunek Where do zapytania, zawężone do znalezionych albumów.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(normalizedSearch) ||
                a.AlbumArtists.Any(aa => aa.Artist.Name.ToLower().Contains(normalizedSearch)));
        }

        // Gdy genreId != null, filtruje albumy po gatunku muzycznym.
        if (genreId.HasValue)
        {
            query = query.Where(a => a.AlbumGenres.Any(ag => ag.GenreId == genreId.Value));
        }

        // Mapuje do DTO i wykonuje zapytanie SQL.
        return await query
            // Sortuje albumy alfabetycznie po nazwie.
            .OrderBy(a => a.Title)
            .Select(a => new AlbumListItemDto  
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                CoverUrl = a.CoverUrl,
                Artists = string.Join(", ", a.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)  // łączy artystów w jednego stringa 
                    .Select(aa => aa.Artist.Name)),
                Genres = string.Join(", ", a.AlbumGenres
                    .Select(ag => ag.Genre.Name)  // łączy gatunki w jednego stringa 
                    .OrderBy(name => name))
            })
            .ToListAsync();  // wykonuje zapytanie do bazy.
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera z bazy dane albumu o podanym Id.
    /// </summary>
    /// <param name="id">Id albumu</param>
    /// <returns>Zwraca obiekt AlbumDetailsDto gdy znajdzie go w bazie, w przeciwnym razie null.</returns>
    public async Task<AlbumDetailsDto?> GetByIdAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Album musi mieć podane id oraz należeć do zalogowanego użytkownika.
        return await _context.Albums
            .AsNoTracking()
            .Where(a => a.Id == id && a.UserId == userId)
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
            // Mapowanie do DTO
            .Select(a => new AlbumDetailsDto
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                CoverUrl = a.CoverUrl,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                ArtistIds = a.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)
                    .Select(aa => aa.ArtistId)
                    .ToList(),
                ArtistNames = a.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)
                    .Select(aa => aa.Artist.Name)
                    .ToList(),
                GenreIds = a.AlbumGenres
                    .Select(ag => ag.GenreId)
                    .ToList(),
                GenreNames = a.AlbumGenres
                    .Select(ag => ag.Genre.Name)
                    .OrderBy(name => name)
                    .ToList()
            })
            .FirstOrDefaultAsync(); // Pobiera pierwszy lub jedyny wynik, w przeciwnym razie null.
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Tworzy nowy album w bazie danych.
    /// </summary>
    /// <param name="dto">Obiekt AlbumCreateDto, zawierający pola potrzebne do stworzenia rekordu w bazie.</param>
    /// <returns>Id utworzonego albumu.</returns>
    public async Task<int> CreateAsync(AlbumCreateDto dto)
    {
        var userId = GetCurrentUserIdOrThrow();
        
        // Walidacja i normalizacja danych z DTO.
        var normalizedTitle = NormalizeTitle(dto.Title);
        ValidateTitle(normalizedTitle);  
        ValidateReleaseYear(dto.ReleaseYear);
        var normalizedCoverUrl = NormalizeOptional(dto.CoverUrl, 500, "Cover URL");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");
        var artistIds = NormalizeIdList(dto.ArtistIds, "Artist");
        var genreIds = NormalizeIdList(dto.GenreIds, "Genre");

        // Metody sprawdzające czy artyści i gatunki należą do zalogowanego użytkownika.
        await EnsureArtistsBelongToUserAsync(artistIds, userId);
        await EnsureGenresBelongToUserAsync(genreIds, userId);

        // Tworzenie nowego albumu.
        var album = new Album
        {
            Title = normalizedTitle,
            ReleaseYear = dto.ReleaseYear,
            CoverUrl = normalizedCoverUrl,
            Description = normalizedDescription,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        // Dodanie arystów do albumu zachowując kolejność w jakiej zostali wybrani.
        for (var i = 0; i < artistIds.Count; i++)
        {
            album.AlbumArtists.Add(new AlbumArtist
            {
                ArtistId = artistIds[i],
                DisplayOrder = i + 1
            });
        }

        // dodaje gatunki do albumu.
        foreach (var genreId in genreIds)
        {
            album.AlbumGenres.Add(new AlbumGenre { GenreId = genreId });
        }

        // Rejestruje obiekt w EF Core jako do dodania do bazy. 
        _context.Albums.Add(album);
        // Zapisuje nowy album oraz powiązane z nim encje do bazy.
        await _context.SaveChangesAsync();
        
        return album.Id;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Aktualizuje dane albumu o podanym Id.
    /// </summary>
    /// <param name="id">id albumu do edycji</param>
    /// <param name="dto">AlbumUpdateDto</param>
    /// <returns>Wartość bool, true oznacza poprawne wykonanie operacji, false gdy nie znajdzie albumu.</returns>
    public async Task<bool> UpdateAsync(int id, AlbumUpdateDto dto)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Pobiera album wraz z podpiętymi artystami i gatunkami muzycznymi.
        var album = await _context.Albums
            .Include(a => a.AlbumArtists)
            .Include(a => a.AlbumGenres)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        // zwraca false gdy nie znajdzie albumu
        if (album is null)
            return false;

        // Walidacja i normalizacja danych z DTO.
        var normalizedTitle = NormalizeTitle(dto.Title);
        ValidateTitle(normalizedTitle);
        ValidateReleaseYear(dto.ReleaseYear);
        var normalizedCoverUrl = NormalizeOptional(dto.CoverUrl, 500, "Cover URL");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");
        var artistIds = NormalizeIdList(dto.ArtistIds, "Artist");
        var genreIds = NormalizeIdList(dto.GenreIds, "Genre");

        // Metody sprawdzające czy artyści i gatunki należą do zalogowanego użytkownika.
        await EnsureArtistsBelongToUserAsync(artistIds, userId);
        await EnsureGenresBelongToUserAsync(genreIds, userId);

        // Normalizacja edytowanych danych.
        album.Title = normalizedTitle;
        album.ReleaseYear = dto.ReleaseYear;
        album.CoverUrl = normalizedCoverUrl;
        album.Description = normalizedDescription;

        // Czyści powiązania EF Core z artystami i dodaje nowe.
        album.AlbumArtists.Clear();
        for (var i = 0; i < artistIds.Count; i++)
        {
            album.AlbumArtists.Add(new AlbumArtist
            {
                AlbumId = album.Id,
                ArtistId = artistIds[i],
                DisplayOrder = i + 1
            });
        }

        // Czyści powiązania EF Core z gatunkami i dodaje nowe.
        album.AlbumGenres.Clear();
        foreach (var genreId in genreIds)
        {
            album.AlbumGenres.Add(new AlbumGenre
            {
                AlbumId = album.Id,
                GenreId = genreId
            });
        }

        // Zapisuje zmiany do bazy.
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Usuwa album o podanym Id.
    /// </summary>
    /// <param name="id">Id albumu do usunięcia.</param>
    /// <returns>Wartość bool, true oznacza poprawne wykonanie operacji, false gdy nie znajdzie albumu.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Pobiera album, który ma być usunięty. Sprawdza czy należy do zalogowanego użytkownika.
        var album = await _context.Albums
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (album is null)
            return false;

        // Rejestruje obiekt w EF Core jako do usnięcia z bazy. 
        _context.Albums.Remove(album);
        // Zapisuje zmiany do bazy.
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Przekazuje Id aktualnie zalogowanego użytkownika, w przeciwnym razie wyrzuca wyjątek.
    /// </summary>
    /// <returns>Id zalogowanego użytkownika</returns>
    /// <exception cref="UnauthorizedAccessException">Gdy użytkownik nie jest zalogowany.</exception>
    private int GetCurrentUserIdOrThrow()
        => _currentUser.UserId ?? throw new UnauthorizedAccessException("Użytkownik nie jest zalogowany.");

    /// <summary>
    /// Normalizacja tytułu albumu. Zamienia litery na małe i usuwa spacje z początku i końca.
    /// </summary>
    /// <param name="title">Tytuł albumu</param>
    /// <returns>Znormalizowany tytuł albumu.</returns>
    private static string NormalizeTitle(string title) => title.Trim();

    /// <summary>
    /// Walidacja tytułu albumu.
    /// Tytuł nie może być pusty, max 150 znaków.
    /// </summary>
    /// <param name="title">Tytuł albumu.</param>
    /// <exception cref="ArgumentException">Pusty tytuł lub więcej niż 150 znaków.</exception>
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Tytuł jest wymagany.");
        if (title.Length > 150)
            throw new ArgumentException("Tytuł nie może mieć więcej niż 150 znaków.");
    }

    /// <summary>
    /// Walidacja roku wydania albumu.
    /// </summary>
    /// <param name="releaseYear">Rok wydania albumu.</param>
    /// <exception cref="ArgumentException">Gdy rok ma wartość poza 1000-aktualny rok.</exception>
    private static void ValidateReleaseYear(int? releaseYear)
    {
        if (releaseYear is < 1000 || releaseYear > DateTime.UtcNow.Year)
            throw new ArgumentException("Data wydania między 1000 a aktualnym rokiem.");
    }

    /// <summary>
    /// Normalizuje dane z opcjonalnego pola.
    /// </summary>
    /// <param name="value">Parametr string</param>
    /// <param name="maxLength">Max długość znaków.</param>
    /// <param name="fieldName">Nazwa uzupełnianego pola.</param>
    /// <returns>Parametr po normalizacji, gdy parametr wejściowy jest pusty zwraca null.</returns>
    /// <exception cref="ArgumentException">Zwraca wyjątek, mówiący o zbyt długiej ilości znaków.</exception>
    private static string? NormalizeOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{fieldName} musi mieć max {maxLength} znaków.");
        return trimmed;
    }

    /// <summary>
    /// Normalizuje dane w liście typu int. 
    /// </summary>
    /// <param name="ids">Parametr do iterowania po liście.</param>
    /// <param name="fieldName">Nazwa uzupełnianego pola.</param>
    /// <returns>Znormalizowana lista typu int.</returns>
    /// <exception cref="ArgumentException">Zwraca komunikat o wyjątku, kiedy Id jest ujemne.</exception>
    private static List<int> NormalizeIdList(IEnumerable<int> ids, string fieldName)
    {
        var result = new List<int>();
        var seen = new HashSet<int>();  // HashSet<int> to zbiór który nie pozwala na duplikaty.
        foreach (var id in ids)
        {
            if (id <= 0)
                throw new ArgumentException($"{fieldName}Id musi być większe od zera.");
            // sprawdza czy jest duplikat, jeśli nie to dodaje do zbioru.
            if (seen.Add(id))
                result.Add(id);  
        }
        return result;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Sprawdza czy Artyści w albumie należą do zalogowanego użytkownika.
    /// </summary>
    /// <param name="artistIds">Lista id artystów należących do albumu, dostępna do odczytu.</param>
    /// <param name="userId">Id użytkownika.</param>
    /// <exception cref="InvalidOperationException">Komunikat o wyjątku, kiedy artysta nie należy do użytkownika.</exception>
    private async Task EnsureArtistsBelongToUserAsync(IReadOnlyCollection<int> artistIds, int userId)
    {
        if (artistIds.Count == 0) return;
        // Liczy ilu artystów z podanej listy istnieje w bazie i należy do zalogowanego użytkownika.
        var existingCount = await _context.Artists
            .CountAsync(a => artistIds.Contains(a.Id) && a.UserId == userId);
        // Jeśli lista id nie zgadza się z wyliczeniami, zwraca wyjątek.
        if (existingCount != artistIds.Count)
            throw new InvalidOperationException("Artysta nie istnieje.");
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Sprawdza czy gatunki muzyczne w albumie należą do zalogowanego użytkownika.
    /// </summary>
    /// <param name="genreIds">Lista id gatunków muzycznych, dostępna do odczytu.</param>
    /// <param name="userId">Id użytkownika.</param>
    /// <exception cref="InvalidOperationException">Komunikat o wyjątku, kiedy gatunek nie należy do użytkownika.</exception>
    private async Task EnsureGenresBelongToUserAsync(IReadOnlyCollection<int> genreIds, int userId)
    {
        if (genreIds.Count == 0) return;
        var existingCount = await _context.Genres
            .CountAsync(g => genreIds.Contains(g.Id) && g.UserId == userId);
        if (existingCount != genreIds.Count)
            throw new InvalidOperationException("Gatunek nie istnieje.");
    }
}
