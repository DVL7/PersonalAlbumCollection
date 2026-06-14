// GenreService.cs

using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.Services.Interfaces;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Implementations;

/// <summary>
/// Serwis zawierający metody do pracy na gatunkach muzycznych.
/// Wszystkie operacje są izolowane do zalogowanego użytkownika.
/// </summary>
public class GenreService : IGenreService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="context">Kontekst bazy danych wstrzykiwany przez DI.</param>
    /// <param name="currentUser">Serwis dostarczający Id zalogowanego użytkownika.</param>
    public GenreService(AppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Pobiera wszystkie gatunki muzyczne należące do zalogowanego użytkownika.
    /// </summary>
    /// <returns>Lista obiektów GenreDto posortowana alfabetycznie.</returns>
    public async Task<List<GenreDto>> GetAllAsync()
    {
        var userId = GetCurrentUserIdOrThrow();

        // Filtruje gatunki należące do zalogowanego użytkownika,
        // sortuje alfabetycznie, mapuje do DTO i wykonuje zapytanie SQL.
        return await _context.Genres
            .AsNoTracking()
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                AlbumCount = g.AlbumGenres.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Pobiera gatunek muzyczny o podanym Id należący do zalogowanego użytkownika.
    /// </summary>
    /// <param name="id">Id gatunku.</param>
    /// <returns>Obiekt GenreDto lub null gdy nie znajdzie.</returns>
    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Gatunek musi należeć do zalogowanego użytkownika.
        return await _context.Genres
            .AsNoTracking()
            .Where(g => g.Id == id && g.UserId == userId)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                AlbumCount = g.AlbumGenres.Count
            })
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Tworzy nowy gatunek muzyczny w kolekcji zalogowanego użytkownika.
    /// </summary>
    /// <param name="name">Nazwa nowego gatunku.</param>
    /// <returns>Id utworzonego gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy gatunek o takiej nazwie już istnieje.</exception>
    public async Task<int> CreateAsync(string name)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Walidacja i normalizacja nazwy.
        name = NormalizeName(name);
        ValidateName(name);

        // Sprawdzenie unikalności nazwy w obrębie kolekcji użytkownika.
        var exists = await _context.Genres
            .AsNoTracking()
            .AnyAsync(g => g.UserId == userId && g.Name.ToLower() == name.ToLower());

        if (exists)
            throw new InvalidOperationException("Gatunek o takiej nazwie już istnieje.");

        var genre = new Genre
        {
            Name = name,
            UserId = userId
        };

        // Rejestruje obiekt w EF Core jako nowy rekord do dodania.
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return genre.Id;
    }

    /// <summary>
    /// Aktualizuje nazwę gatunku o podanym Id.
    /// </summary>
    /// <param name="id">Id gatunku do edycji.</param>
    /// <param name="newName">Nowa nazwa gatunku.</param>
    /// <returns>True gdy aktualizacja się powiodła, false gdy nie znaleziono gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy inna nazwa jest już zajęta.</exception>
    public async Task<bool> UpdateAsync(int id, string newName)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Walidacja i normalizacja nazwy.
        newName = NormalizeName(newName);
        ValidateName(newName);

        // Celowo bez AsNoTracking — EF Core musi śledzić obiekt żeby wykonać UPDATE.
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (genre is null)
            return false;

        // Sprawdzenie unikalności nazwy — pomija aktualnie edytowany gatunek.
        var nameTaken = await _context.Genres
            .AnyAsync(g => g.UserId == userId && g.Id != id && g.Name.ToLower() == newName.ToLower());

        if (nameTaken)
            throw new InvalidOperationException("Inny gatunek o takiej nazwie już istnieje.");

        genre.Name = newName;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Usuwa gatunek muzyczny o podanym Id.
    /// </summary>
    /// <param name="id">Id gatunku do usunięcia.</param>
    /// <returns>True gdy usunięcie się powiodło, false gdy nie znaleziono gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy gatunek jest przypisany do albumów.</exception>
    public async Task<bool> DeleteAsync(int id)
    {
        var userId = GetCurrentUserIdOrThrow();

        // Celowo bez AsNoTracking — EF Core musi śledzić obiekt żeby wykonać DELETE.
        var genre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (genre is null)
            return false;

        // Zabezpieczenie przed usunięciem gatunku przypisanego do albumów.
        var isUsed = await _context.AlbumGenres
            .AsNoTracking()
            .AnyAsync(ag => ag.GenreId == id);

        if (isUsed)
            throw new InvalidOperationException("Nie można usunąć gatunku, który jest przypisany do albumów.");

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Zwraca Id zalogowanego użytkownika lub rzuca wyjątek gdy nikt nie jest zalogowany.
    /// </summary>
    /// <returns>Id zalogowanego użytkownika.</returns>
    /// <exception cref="UnauthorizedAccessException">Gdy użytkownik nie jest zalogowany.</exception>
    private int GetCurrentUserIdOrThrow()
        => _currentUser.UserId ?? throw new UnauthorizedAccessException("Użytkownik nie jest zalogowany.");

    /// <summary>
    /// Usuwa białe znaki z początku i końca nazwy.
    /// </summary>
    /// <param name="name">Nazwa gatunku.</param>
    /// <returns>Znormalizowana nazwa bez białych znaków.</returns>
    private static string NormalizeName(string name) => name.Trim();

    /// <summary>
    /// Waliduje nazwę gatunku — musi mieć od 2 do 50 znaków.
    /// </summary>
    /// <param name="name">Nazwa gatunku.</param>
    /// <exception cref="ArgumentException">Gdy nazwa jest pusta, za krótka lub za długa.</exception>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nazwa nie może być pusta.");
        if (name.Length < 2)
            throw new ArgumentException("Nazwa musi składać się z przynajmniej dwóch znaków.");
        if (name.Length > 50)
            throw new ArgumentException("Nazwa może posiadać co najwyżej 50 znaków.");
    }
}
