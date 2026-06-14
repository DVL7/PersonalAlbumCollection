// GenreDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący dane gatunku muzycznego, wyłączając właściwości nawigacyjne EF Core.
/// Używany do CRUDa oraz wyświetlania danych.
/// Używany przez GenreService i widoki Genres, Albums, AlbumCreate, AlbumEdit.
/// </summary>
public class GenreDto
{
    /// <summary>
    /// Id gatunku muzycznego.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nazwa gatunku muzycznego.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Liczba albumów powiązanych z gatunkiem.
    /// Wyliczana przez GenreService, używana w widoku Genres.
    /// </summary>
    public int AlbumCount { get; set; }
}