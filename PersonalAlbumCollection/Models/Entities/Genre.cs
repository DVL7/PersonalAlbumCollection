namespace PersonalAlbumCollection.Models.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();
}