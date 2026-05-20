using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.Models.Entities;

public class UserAlbum
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public int? Score { get; set; }

    public string? Review { get; set; }

    public UserAlbumStatus Status { get; set; } = UserAlbumStatus.Completed;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}