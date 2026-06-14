// AlbumGenre.cs

namespace PersonalAlbumCollection.Models.Entities;

/// <summary>
/// Tabela łącząca Album i Genre.
/// Relacja wiele do wielu.
/// Klucz złożony z AlbumId i GenreId.
/// </summary>
public class AlbumGenre
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
    /// Klucz obcy do tabeli Genre. Część klucza złożonego.
    /// </summary>
    public int GenreId { get; set; }
    /// <summary>
    /// Właściwość nawigacyjna do powiązanego gatunku muzycznego.
    /// Wypełniana przez EF Core przy użyciu Include().
    /// </summary>
    public Genre Genre { get; set; } = null!;
}