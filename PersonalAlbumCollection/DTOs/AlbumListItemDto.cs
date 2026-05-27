namespace PersonalAlbumCollection.DTOs;

public class AlbumListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ReleaseYear { get; set; }
    public string? CoverUrl { get; set; }
    public string Artists { get; set; } = string.Empty;
    public string Genres { get; set; } = string.Empty;
}