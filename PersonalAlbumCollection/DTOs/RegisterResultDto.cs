namespace PersonalAlbumCollection.DTOs;

public class RegisterResultDto
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? ErrorMessage { get; set; }
}
