namespace PersonalAlbumCollection.DTOs;

public class AlbumUpdateDto
{
    public string Title { get; set; } = string.Empty;
    public int? ReleaseYear { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }

    public List<int> ArtistIds { get; set; } = new();
    public List<int> GenreIds { get; set; } = new();
}