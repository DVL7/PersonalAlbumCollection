// IArtistService.cs

using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

/// <summary>
/// Definiuje operacje dostępne na artystach w kolekcji użytkownika.
/// Wszystkie operacje są izolowane do zalogowanego użytkownika.
/// </summary>
public interface IArtistService
{
    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera wszystkich artystów należących do zalogowanego użytkownika.
    /// </summary>
    /// <param name="search">Opcjonalna fraza wyszukiwania po nazwie artysty.</param>
    /// <returns>Lista obiektów ArtistDto.</returns>
    Task<List<ArtistDto>> GetAllAsync(string? search = null);

    /// <summary>
    /// Metoda asynchroniczna.
    /// Pobiera artystę o podanym Id należącego do zalogowanego użytkownika.
    /// </summary>
    /// <param name="id">Id artysty.</param>
    /// <returns>Obiekt ArtistDto lub null gdy nie znajdzie.</returns>
    Task<ArtistDto?> GetByIdAsync(int id);

    /// <summary>
    /// Metoda asynchroniczna.
    /// Tworzy nowego artystę w kolekcji zalogowanego użytkownika.
    /// </summary>
    /// <param name="dto">Dane nowego artysty.</param>
    /// <returns>Id utworzonego artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy artysta o takiej nazwie już istnieje.</exception>
    Task<int> CreateAsync(ArtistCreateDto dto);

    /// <summary>
    /// Metoda asynchroniczna.
    /// Aktualizuje dane artysty o podanym Id.
    /// </summary>
    /// <param name="id">Id artysty do edycji.</param>
    /// <param name="dto">Nowe dane artysty.</param>
    /// <returns>True gdy aktualizacja się powiodła, false gdy nie znaleziono artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy inna nazwa jest już zajęta.</exception>
    Task<bool> UpdateAsync(int id, ArtistUpdateDto dto);

    /// <summary>
    /// Usuwa artystę o podanym Id.
    /// </summary>
    /// <param name="id">Id artysty do usunięcia.</param>
    /// <returns>True gdy usunięcie się powiodło, false gdy nie znaleziono artysty.</returns>
    /// <exception cref="InvalidOperationException">Gdy artysta jest przypisany do albumów.</exception>
    Task<bool> DeleteAsync(int id);
}