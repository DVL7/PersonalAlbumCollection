// ArtistService.cs

using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

/// <summary>
/// Serwis zawierający metody do pracy na artystach.
/// Wszystkie operacje są izolowane do zalogowanego użytkownika.
/// </summary>
public class ArtistService : IArtistService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="context">Kontekst bazy danych wstrzykiwany przez DI.</param>
    /// <param name="currentUser">Serwis dostarczający Id zalogowanego użytkownika.</param>
    public ArtistService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera wszystkich artystów należących do zalogowanego użytkownika.
    /// </summary>
    /// <param name="search">Opcjonalna fraza wyszukiwania po nazwie artysty.</param>
    /// <returns>Lista obiektów ArtistDto.</returns>
    public async Task<List<ArtistDto>> GetAllAsync(string? search = null)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Filtruje artystów należących do zalogowanego użytkownika.
        var query = _context.Artists
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .AsQueryable();

        // Gdy search != null, zawęża zapytanie do artystów których nazwa zawiera podaną frazę.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(normalizedSearch));
        }

        // Sortuje alfabetycznie, mapuje do DTO i wykonuje zapytanie SQL.
        return await query
            .OrderBy(a => a.Name)
            .Select(a => new ArtistDto
            {
                Id = a.Id,
                Name = a.Name,
                ArtistType = a.ArtistType,
                Country = a.Country,
                Description = a.Description,
                AlbumCount = a.AlbumArtists.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera artystę o podanym Id należącego do zalogowanego użytkownika.
    /// </summary>
    /// <param name="id">Id artysty.</param>
    /// <returns>Obiekt ArtistDto lub null gdy nie znajdzie.</returns>
    public async Task<ArtistDto?> GetByIdAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Artysta musi należeć do zalogowanego użytkownika.
        return await _context.Artists
            .AsNoTracking()
            .Where(a => a.Id == id && a.UserId == userId)
            .Select(a => new ArtistDto
            {
                Id = a.Id,
                Name = a.Name,
                ArtistType = a.ArtistType,
                Country = a.Country,
                Description = a.Description,
                AlbumCount = a.AlbumArtists.Count
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Tworzy nowego artystę w kolekcji zalogowanego użytkownika.
    /// </summary>
    /// <param name="dto">Dane nowego artysty.</param>
    /// <returns>Id utworzonego artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy artysta o takiej nazwie już istnieje.</exception>
    public async Task<int> CreateAsync(ArtistCreateDto dto)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Walidacja i normalizacja danych z DTO.
        var normalizedName = NormalizeName(dto.Name);
        ValidateName(normalizedName);
        var normalizedCountry = NormalizeOptional(dto.Country, 50, "Country");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");

        // Sprawdzenie unikalności nazwy w obrębie kolekcji użytkownika.
        var nameTaken = await _context.Artists
            .AnyAsync(a => a.UserId == userId && a.Name.ToLower() == normalizedName.ToLower());

        if (nameTaken)
            throw new InvalidOperationException("Artysta o takiej nazwie już istnieje.");

        var artist = new Artist
        {
            Name = normalizedName,
            ArtistType = dto.ArtistType,
            Country = normalizedCountry,
            Description = normalizedDescription,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        return artist.Id;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Aktualizuje dane artysty o podanym Id.
    /// </summary>
    /// <param name="id">Id artysty do edycji.</param>
    /// <param name="dto">Nowe dane artysty.</param>
    /// <returns>True gdy aktualizacja się powiodła, false gdy nie znaleziono artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy inna nazwa jest już zajęta.</exception>
    public async Task<bool> UpdateAsync(int id, ArtistUpdateDto dto)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Celowo bez AsNoTracking — EF Core musi śledzić obiekt żeby wykonać UPDATE.
        var artist = await _context.Artists
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (artist is null)
            return false;

        // Walidacja i normalizacja danych z DTO.
        var normalizedName = NormalizeName(dto.Name);
        ValidateName(normalizedName);
        var normalizedCountry = NormalizeOptional(dto.Country, 50, "Country");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");

        // Sprawdzenie unikalności nazwy — pomija aktualnie edytowanego artystę.
        var nameTaken = await _context.Artists
            .AnyAsync(a => a.UserId == userId && a.Id != id && a.Name.ToLower() == normalizedName.ToLower());

        if (nameTaken)
            throw new InvalidOperationException("Inny artysta o takiej nazwie już istnieje.");

        artist.Name = normalizedName;
        artist.ArtistType = dto.ArtistType;
        artist.Country = normalizedCountry;
        artist.Description = normalizedDescription;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Usuwa artystę o podanym Id.
    /// </summary>
    /// <param name="id">Id artysty do usunięcia.</param>
    /// <returns>True gdy usunięcie się powiodło, false gdy nie znaleziono artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy artysta jest przypisany do albumów.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Include potrzebny do sprawdzenia czy artysta ma przypisane albumy.
        var artist = await _context.Artists
            .Include(a => a.AlbumArtists)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (artist is null)
            return false;

        // Zabezpieczenie przed usunięciem artysty przypisanego do albumów.
        if (artist.AlbumArtists.Any())
            throw new InvalidOperationException("Nie można usunąć artysty podpiętego pod album.");

        _context.Artists.Remove(artist);
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
    /// Usuwa białe znaki z początku i końca nazwy.
    /// </summary>
    /// <param name="name">Nazwa artsty/zespołu.</param>
    /// <returns>Znormalizowana nazwa bez białych znaków.</returns>
    private static string NormalizeName(string name) => name.Trim();

    /// <summary>
    /// Waliduje nazwę artysty — nie może być pusta i mieć maksymalnie 50 znaków.
    /// </summary>
    /// <param name="name">Nazwa artysty/zespołu.</param>
    /// <exception cref="ArgumentException">Gdy nazwa jest pusta lub za długa.</exception>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nazwa jest wymagana.");
        if (name.Length > 50)
            throw new ArgumentException("Name nie może mieć więcej niż 50 znaków.");
    }

    /// <summary>
    /// Normalizuje opcjonalne pole tekstowe — przycina spacje i sprawdza długość.
    /// </summary>
    /// <param name="value">Parametr string.</param>
    /// <param name="maxLength">Max długość znaków.</param>
    /// <param name="fieldName">Nazwa uzupełnianego pola.</param>
    /// <returns>Parametr po normalizacji, gdy parametr wejściowy jest pusty zwraca null.</returns>
    /// <exception cref="ArgumentException">Gdy wartość przekracza maksymalną długość.</exception>
    private static string? NormalizeOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"{fieldName} musi mieć max {maxLength} znaków.");
        return trimmed;
    }
}
