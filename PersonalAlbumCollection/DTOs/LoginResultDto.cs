namespace PersonalAlbumCollection.DTOs;

public class LoginResultDto
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? ErrorMessage { get; set; }
}