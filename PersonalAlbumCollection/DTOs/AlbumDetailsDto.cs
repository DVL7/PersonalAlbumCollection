// AlbumDetailsDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący dane albumu, wyłączając właściwości nawigacyjne EF Core.
/// Używane przez AlbumService i widoki AlbumDetails, AlbumEdit.
/// </summary>
public class AlbumDetailsDto
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
    public string? Description { get; set; }
    /// <summary>
    /// Data utworzenia albumu.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Lista Id artystów powiązanych z albumem.
    /// </summary>
    public List<int> ArtistIds { get; set; } = new();
    /// <summary>
    /// Lista nazw artystów powiązanych z albumem.
    /// </summary>
    public List<string> ArtistNames { get; set; } = new();

    /// <summary>
    /// Lista Id gatunków muzycznych powiązanych z albumem.
    /// </summary>
    public List<int> GenreIds { get; set; } = new();
    /// <summary>
    /// Lista nazw gatunków muzycznych powiązanych z albumem.
    /// </summary>
    public List<string> GenreNames { get; set; } = new(); 
}