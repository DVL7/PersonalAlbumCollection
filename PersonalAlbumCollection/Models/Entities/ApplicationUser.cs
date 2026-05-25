namespace PersonalAlbumCollection.Models.Entities;

public class ApplicationUser 
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserAlbum> UserAlbums { get; set; } = new List<UserAlbum>();
}