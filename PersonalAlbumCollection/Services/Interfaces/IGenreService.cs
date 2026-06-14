// IGenreService.cs

using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

/// <summary>
/// Serwis zawierający metody do pracy na gatunkach muzycznych.
/// Wszystkie operacje są izolowane do zalogowanego użytkownika.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Pobiera wszystkie gatunki muzyczne należące do zalogowanego użytkownika.
    /// </summary>
    /// <returns>Lista obiektów GenreDto posortowana alfabetycznie.</returns>
    Task<List<GenreDto>> GetAllAsync();
    /// <summary>
    /// Pobiera gatunek muzyczny o podanym Id należący do zalogowanego użytkownika.
    /// </summary>
    /// <param name="id">Id gatunku.</param>
    /// <returns>Obiekt GenreDto lub null gdy nie znajdzie.</returns>
    Task<GenreDto?> GetByIdAsync(int id);
    /// <summary>
    /// Tworzy nowy gatunek muzyczny w kolekcji zalogowanego użytkownika.
    /// </summary>
    /// <param name="name">Nazwa nowego gatunku.</param>
    /// <returns>Id utworzonego gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy gatunek o takiej nazwie już istnieje.</exception>
    Task<int> CreateAsync(string name);
    /// <summary>
    /// Aktualizuje nazwę gatunku o podanym Id.
    /// </summary>
    /// <param name="id">Id gatunku do edycji.</param>
    /// <param name="newName">Nowa nazwa gatunku.</param>
    /// <returns>True gdy aktualizacja się powiodła, false gdy nie znaleziono gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy inna nazwa jest już zajęta.</exception>
    Task<bool> UpdateAsync(int id, string newName);
    /// <summary>
    /// Usuwa gatunek muzyczny o podanym Id.
    /// </summary>
    /// <param name="id">Id gatunku do usunięcia.</param>
    /// <returns>True gdy usunięcie się powiodło, false gdy nie znaleziono gatunku.</returns>
    /// <exception cref="InvalidOperationException">Gdy gatunek jest przypisany do albumów.</exception>
    Task<bool> DeleteAsync(int id);
    
}