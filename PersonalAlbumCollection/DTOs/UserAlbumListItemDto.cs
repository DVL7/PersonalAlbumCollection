using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.DTOs;

public class UserAlbumListItemDto
{
    public int AlbumId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ReleaseYear { get; set; }
    public string? CoverUrl { get; set; }
    public string Artists { get; set; } = string.Empty;
    public string Genres { get; set; } = string.Empty;
    public int? Score { get; set; }
    public string? Review { get; set; }
    public UserAlbumStatus Status { get; set; }
    public DateTime AddedAt { get; set; }
}