namespace PersonalAlbumCollection.Models.Entities;

public class Album
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }

    public string? CoverUrl { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

    public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();

    public ICollection<UserAlbum> UserAlbums { get; set; } = new List<UserAlbum>();
}