// AlbumArtist.cs


namespace PersonalAlbumCollection.Models.Entities;

/// <summary>
/// Tabela łącząca Album i Artist.
/// Relacja wiele do wielu.
/// Klucz złożony z AlbumId i ArtistId.
/// </summary>

public class AlbumArtist
{
    /// <summary>
    /// Klucz obcy do tabeli Album. Część klucza złożonego.
    /// </summary>
    public int AlbumId { get; set; }
    /// <summary>
    /// Właściwość nawigacyjna do powiązanego albumu.
    /// Wypełniana przez EF Core przy użyciu Include().
    /// </summary>
    public Album Album { get; set; } = null!;
    /// <summary>
    /// Klucz obcy do tabeli Artists. Część klucza złożonego.
    /// </summary>
    public int ArtistId { get; set; }
    /// <summary>
    /// Właściwość nawigacyjna do powiązanego artysty.
    /// Wypełniana przez EF Core przy użyciu Include().
    /// </summary>
    public Artist Artist { get; set; } = null!;
    /// <summary>
    /// Kolejność wyświetlania artysty na albumie (np. 1 = główny artysta).
    /// Używana przy sortowaniu listy artystów albumu.
    /// </summary>
    public int DisplayOrder { get; set; }
}