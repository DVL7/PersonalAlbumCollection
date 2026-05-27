using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.Services.Interfaces;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly AppDbContext _context;

    public GenreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenreDto>> GetAllAsync()
    {
        return await _context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                AlbumCount = g.AlbumGenres.Count
            })
            .ToListAsync();
    }

    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        return await _context.Genres
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                AlbumCount = g.AlbumGenres.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(string name)
    {
        name = NormalizeName(name);
        
        ValidateName(name);
        
        var exists = await ExistsByNameAsync(name);

        if (exists)
        {
            throw new InvalidOperationException("Gatunek o takiej nazwie już istnieje.");
        }

        var genre = new Genre
        {
            Name = name
        };
        
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        
        return genre.Id;
    }

    public async Task<bool> UpdateAsync(int id, string newName)
    {
        newName = NormalizeName(newName);

        ValidateName(newName);

        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return false;
        }

        var nameTaken = await _context.Genres
            .AnyAsync(g => g.Id != id && g.Name.ToLower() == newName.ToLower());

        if (nameTaken)
        {
            throw new InvalidOperationException("Inny gatunek o takiej nazwie już istnieje.");
        }

        genre.Name = newName;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
        {
            return false;
        }

        var isUsed = await IsUsedByAnyAlbumAsync(id);

        if (isUsed)
        {
            throw new InvalidOperationException("Nie można usunąć gatunku, który jest przypisany do albumów.");
        }

        _context.Genres.Remove(genre);
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
            throw new ArgumentException("Nazwa nie może być pusta!");
        }
        if (name.Length < 2)
        {
            throw new ArgumentException("Nazwa musi składać się z przynajmniej dwóch znaków.");
        }
        if (name.Length > 50)
        {
            throw new ArgumentException("Nazwa może posiadać co najwyżej 50 znaków.");
        }
    }

    private async Task<bool> ExistsByNameAsync(string name)
    {
        name = NormalizeName(name);

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        
        return await _context.Genres
            .AsNoTracking()
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());
    }
    
    private async Task<bool> IsUsedByAnyAlbumAsync(int id)
    {
        return await _context.AlbumGenres
            .AsNoTracking()
            .AnyAsync(ag => ag.GenreId == id);
    }
    
}