namespace PersonalAlbumCollection.Models.Entities;

public class AlbumGenre
{
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}