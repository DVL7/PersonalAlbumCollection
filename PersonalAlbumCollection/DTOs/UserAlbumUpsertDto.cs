using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.DTOs;

public class UserAlbumUpsertDto
{
    public int AlbumId { get; set; }
    public int? Score { get; set; }
    public string? Review { get; set; }
    public UserAlbumStatus Status { get; set; } = UserAlbumStatus.Completed;
}