using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.Models.Enums;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly AppDbContext _context;

    public ArtistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArtistDto>> GetAllAsync(string? search = null)
    {
        var query = _context.Artists
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(normalizedSearch));
        }

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

    public async Task<ArtistDto?> GetByIdAsync(int id)
    {
        return await _context.Artists
            .AsNoTracking()
            .Where(a => a.Id == id)
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

    public async Task<int> CreateAsync(string name, ArtistType artistType, string? country = null, string? description = null)
    {
        var normalizedName = NormalizeName(name);
        ValidateName(normalizedName);
        var normalizedCountry = NormalizeOptional(country, 50, "Country");
        var normalizedDescription = NormalizeOptional(description, 2000, "Description");

        var artist = new Artist
        {
            Name = normalizedName,
            ArtistType = artistType,
            Country = normalizedCountry,
            Description = normalizedDescription,
            CreatedAt = DateTime.UtcNow
        };

        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        return artist.Id;
    }

    public async Task<bool> UpdateAsync(int id, string name, ArtistType artistType, string? country = null, string? description = null)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist is null)
        {
            return false;
        }

        var normalizedName = NormalizeName(name);
        ValidateName(normalizedName);
        var normalizedCountry = NormalizeOptional(country, 50, "Country");
        var normalizedDescription = NormalizeOptional(description, 2000, "Description");

        artist.Name = normalizedName;
        artist.ArtistType = artistType;
        artist.Country = normalizedCountry;
        artist.Description = normalizedDescription;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var artist = await _context.Artists
            .Include(a => a.AlbumArtists)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (artist is null)
        {
            return false;
        }

        if (artist.AlbumArtists.Any())
        {
            throw new InvalidOperationException("Cannot delete artist that is assigned to albums.");
        }

        _context.Artists.Remove(artist);
        await _context.SaveChangesAsync();

        return true;
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.");
        }

        if (name.Length > 50)
        {
            throw new ArgumentException("Name can have at most 50 characters.");
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
}
