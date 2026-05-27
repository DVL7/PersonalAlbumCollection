using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

public class AlbumService : IAlbumService
{
    private readonly AppDbContext _context;

    public AlbumService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlbumListItemDto>> GetAllAsync(string? search = null, int? genreId = null)
    {
        var query = _context.Albums
            .AsNoTracking()
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(a =>
                a.Title.ToLower().Contains(normalizedSearch) ||
                a.AlbumArtists.Any(aa => aa.Artist.Name.ToLower().Contains(normalizedSearch)));
        }

        if (genreId.HasValue)
        {
            query = query.Where(a => a.AlbumGenres.Any(ag => ag.GenreId == genreId.Value));
        }

        return await query
            .OrderBy(a => a.Title)
            .Select(a => new AlbumListItemDto
            {
                Id = a.Id,
                Title = a.Title,
                ReleaseYear = a.ReleaseYear,
                CoverUrl = a.CoverUrl,
                Artists = string.Join(", ", a.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)
                    .Select(aa => aa.Artist.Name)),
                Genres = string.Join(", ", a.AlbumGenres
                    .Select(ag => ag.Genre.Name)
                    .OrderBy(name => name))
            })
            .ToListAsync();
    }

    public async Task<AlbumDetailsDto?> GetByIdAsync(int id)
    {
        return await _context.Albums
            .AsNoTracking()
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.AlbumGenres)
                .ThenInclude(ag => ag.Genre)
            .Where(a => a.Id == id)
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
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(AlbumCreateDto dto)
    {
        var normalizedTitle = NormalizeTitle(dto.Title);
        ValidateTitle(normalizedTitle);
        ValidateReleaseYear(dto.ReleaseYear);
        var normalizedCoverUrl = NormalizeOptional(dto.CoverUrl, 500, "Cover URL");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");
        var artistIds = NormalizeIdList(dto.ArtistIds, "Artist");
        var genreIds = NormalizeIdList(dto.GenreIds, "Genre");

        await EnsureArtistsExistAsync(artistIds);
        await EnsureGenresExistAsync(genreIds);

        var album = new Album
        {
            Title = normalizedTitle,
            ReleaseYear = dto.ReleaseYear,
            CoverUrl = normalizedCoverUrl,
            Description = normalizedDescription,
            CreatedAt = DateTime.UtcNow
        };

        for (var i = 0; i < artistIds.Count; i++)
        {
            album.AlbumArtists.Add(new AlbumArtist
            {
                ArtistId = artistIds[i],
                DisplayOrder = i + 1
            });
        }

        foreach (var genreId in genreIds)
        {
            album.AlbumGenres.Add(new AlbumGenre
            {
                GenreId = genreId
            });
        }

        _context.Albums.Add(album);
        await _context.SaveChangesAsync();

        return album.Id;
    }

    public async Task<bool> UpdateAsync(int id, AlbumUpdateDto dto)
    {
        var album = await _context.Albums
            .Include(a => a.AlbumArtists)
            .Include(a => a.AlbumGenres)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (album is null)
        {
            return false;
        }

        var normalizedTitle = NormalizeTitle(dto.Title);
        ValidateTitle(normalizedTitle);
        ValidateReleaseYear(dto.ReleaseYear);
        var normalizedCoverUrl = NormalizeOptional(dto.CoverUrl, 500, "Cover URL");
        var normalizedDescription = NormalizeOptional(dto.Description, 2000, "Description");
        var artistIds = NormalizeIdList(dto.ArtistIds, "Artist");
        var genreIds = NormalizeIdList(dto.GenreIds, "Genre");

        await EnsureArtistsExistAsync(artistIds);
        await EnsureGenresExistAsync(genreIds);

        album.Title = normalizedTitle;
        album.ReleaseYear = dto.ReleaseYear;
        album.CoverUrl = normalizedCoverUrl;
        album.Description = normalizedDescription;

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

        album.AlbumGenres.Clear();
        foreach (var genreId in genreIds)
        {
            album.AlbumGenres.Add(new AlbumGenre
            {
                AlbumId = album.Id,
                GenreId = genreId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var album = await _context.Albums.FindAsync(id);

        if (album is null)
        {
            return false;
        }

        _context.Albums.Remove(album);
        await _context.SaveChangesAsync();

        return true;
    }

    private static string NormalizeTitle(string title)
    {
        return title.Trim();
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (title.Length > 150)
        {
            throw new ArgumentException("Title can have at most 150 characters.");
        }
    }

    private static void ValidateReleaseYear(int? releaseYear)
    {
        if (releaseYear is < 1000 or > 9999)
        {
            throw new ArgumentException("Release year must be between 1000 and 9999.");
        }
    }

    private static string? NormalizeOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} can have at most {maxLength} characters.");
        }

        return trimmed;
    }

    private static List<int> NormalizeIdList(IEnumerable<int> ids, string fieldName)
    {
        var result = new List<int>();
        var seen = new HashSet<int>();

        foreach (var id in ids)
        {
            if (id <= 0)
            {
                throw new ArgumentException($"{fieldName}Id must be a positive number.");
            }

            if (seen.Add(id))
            {
                result.Add(id);
            }
        }

        return result;
    }

    private async Task EnsureArtistsExistAsync(IReadOnlyCollection<int> artistIds)
    {
        if (artistIds.Count == 0)
        {
            return;
        }

        var existingCount = await _context.Artists.CountAsync(a => artistIds.Contains(a.Id));

        if (existingCount != artistIds.Count)
        {
            throw new InvalidOperationException("One or more artists do not exist.");
        }
    }

    private async Task EnsureGenresExistAsync(IReadOnlyCollection<int> genreIds)
    {
        if (genreIds.Count == 0)
        {
            return;
        }

        var existingCount = await _context.Genres.CountAsync(g => genreIds.Contains(g.Id));

        if (existingCount != genreIds.Count)
        {
            throw new InvalidOperationException("One or more genres do not exist.");
        }
    }
}
