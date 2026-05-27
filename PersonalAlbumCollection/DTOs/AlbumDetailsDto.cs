namespace PersonalAlbumCollection.DTOs;

public class AlbumDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ReleaseYear { get; set; }
    public string? CoverUrl { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<int> ArtistIds { get; set; } = new();
    public List<string> ArtistNames { get; set; } = new();

    public List<int> GenreIds { get; set; } = new();
    public List<string> GenreNames { get; set; } = new(); 
}