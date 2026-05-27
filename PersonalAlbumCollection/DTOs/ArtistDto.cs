using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.DTOs;

public class ArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ArtistType ArtistType { get; set; }
    public string? Country { get; set; }
    public string? Description { get; set; }
    public int AlbumCount { get; set; }
}