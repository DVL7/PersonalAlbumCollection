// IAlbumService.cs

using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

/// <summary>
/// Interfejs serwisu do pracy z albumami.
/// Definiuje operacje dostępne na albumach w kolekcji użytkownika.
/// </summary>
public interface IAlbumService
{
    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera z bazy dane wszystkich albumów należących do zalogowanego użytkownika.
    /// </summary>
    /// <param name="search">Wyszukiwanie po nazwie lub artyście</param>
    /// <param name="genreId">Wyszukiwanie po gatunku muzycznym</param>
    /// <returns> Lista obiektów /<AlbumListItemDto/> </returns>
    Task<List<AlbumListItemDto>> GetAllAsync(string? search = null, int? genreId = null);
    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera z bazy dane albumu o podanym Id.
    /// </summary>
    /// <param name="id">Id albumu</param>
    /// <returns>Zwraca obiekt AlbumDetailsDto gdy znajdzie go w bazie, w przeciwnym razie null.</returns>
    Task<AlbumDetailsDto?> GetByIdAsync(int id);
    /// <summary>
    /// Metoda asynchroniczna.
    /// Tworzy nowy album w bazie danych.
    /// </summary>
    /// <param name="dto">Obiekt AlbumCreateDto, zawierający pola potrzebne do stworzenia rekordu w bazie.</param>
    /// <returns>Id utworzonego albumu.</returns>
    Task<int> CreateAsync(AlbumCreateDto dto);
    /// <summary>
    /// Metoda asynchroniczna.
    /// Aktualizuje dane albumu o podanym Id.
    /// </summary>
    /// <param name="id">id albumu do edycji</param>
    /// <param name="dto">AlbumUpdateDto</param>
    /// <returns>Wartość bool, true oznacza poprawne wykonanie operacji, false gdy nie znajdzie albumu.</returns>
    Task<bool> UpdateAsync(int id, AlbumUpdateDto dto);
    /// <summary>
    /// Metoda asynchroniczna.
    /// Usuwa album o podanym Id.
    /// </summary>
    /// <param name="id">Id albumu do usunięcia.</param>
    /// <returns>Wartość bool, true oznacza poprawne wykonanie operacji, false gdy nie znajdzie albumu.</returns>
    Task<bool> DeleteAsync(int id);
}