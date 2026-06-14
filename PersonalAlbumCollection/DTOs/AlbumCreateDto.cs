// AlbumCreateDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący dane albumu potrzebne do utworzenia obiektu.
/// Nie zawiera pól generowanych automatycznie przy zapisie: Id, CreatedAt.
/// Używane przez AlbumService i widok AlbumCreate.
/// </summary>
public class AlbumCreateDto
{
    /// <summary>
    /// Tytuł albumu.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Rok wydania albumu.
    /// </summary>
    public int? ReleaseYear { get; set; }
    /// <summary>
    /// Adres URL okladki albumu.
    /// </summary>
    public string? CoverUrl { get; set; }
    /// <summary>
    /// Opis lub notatka o albumie.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Lista przechowująca Id powiązanych artystów.
    /// </summary>
    public List<int> ArtistIds { get; set; } = new();
    /// <summary>
    /// Lista przechowująca Id powiązanych gatunków muzycznych.
    /// </summary>
    public List<int> GenreIds { get; set; } = new();

}