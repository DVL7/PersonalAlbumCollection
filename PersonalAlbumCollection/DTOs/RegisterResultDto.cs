// RegisterResultDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// DTO reprezentujące wynik rejestracji użytkownika.
/// Rozszerza AccountResultDto o identyfikator nowo utworzonego użytkownika.
/// Używane przez AuthService.RegisterAsync().
/// </summary>
public class RegisterResultDto : AccountResultDto
{
    /// <summary>
    /// Id nowo utworzonego użytkownika.
    /// Wypełniony gdy Success = true, null gdy rejestracja się nie powiodła.
    /// </summary>
    public int? UserId { get; set; }
}