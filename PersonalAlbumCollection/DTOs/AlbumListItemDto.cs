// AlbumListItemDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący uproszczone dane albumu do wyświetlenia.
/// Używane przez AlbumService.GetAllAsync() i widoki Albums, AlbumCard.
/// </summary>
public class AlbumListItemDto
{
    /// <summary>
    /// Id albumu.
    /// </summary>
    public int Id { get; set; }
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
    public string Artists { get; set; } = string.Empty;
    /// <summary>
    /// Gatunki muzyczne albumu.
    /// </summary>
    public string Genres { get; set; } = string.Empty;
}