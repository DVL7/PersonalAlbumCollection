using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.Models.Entities;

public class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ArtistType ArtistType { get; set; }

    public string? Country { get; set; }

    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
    
}