// ArtistDto.cs

using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący dane artysty/zespołu, wyłączając właściwości nawigacyjne EF Core.
/// Używany do CRUDa oraz wyświetlania danych.
/// Używany przez ArtistService i widoki Artist, AlbumCreate, AlbumEdit.
/// </summary>
public class ArtistDto
{
    /// <summary>
    /// Id artysty/zespołu.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nazwa artysty/zespołu.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Enum określający czy obiekt jest artystą solowym czy zespołem.
    /// </summary>
    public ArtistType ArtistType { get; set; }
    /// <summary>
    /// Kraj powiązany z artystą.
    /// </summary>
    public string? Country { get; set; }
    /// <summary>
    /// Opis lub notatka.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Liczba albumów powiązanych z artystą.
    /// Wyliczana przez ArtistService, używana w widoku Artists.
    /// </summary>
    public int AlbumCount { get; set; }
}