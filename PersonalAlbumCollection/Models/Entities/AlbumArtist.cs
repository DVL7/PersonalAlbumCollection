namespace PersonalAlbumCollection.Models.Entities;

public class AlbumArtist
{
    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public int ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
    
    public int DisplayOrder { get; set; }
}