// LoginResultDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// DTO reprezentujące wynik operacji logowania.
/// Używane przez AuthService.LoginAsync() oraz endpoint /auth/login w Program.cs.
/// </summary>
public class LoginResultDto
{
    /// <summary>
    /// Określa czy logowanie zakończyło się powodzeniem.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Id zalogowanego użytkownika.
    /// Wypełniony gdy Success = true, null w przeciwnym razie.
    /// Używany do ustawienia claimu NameIdentifier w cookie autentykacji.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Nazwa zalogowanego użytkownika.
    /// Wypełniona gdy Success = true, null w przeciwnym razie.
    /// Używana do ustawienia claimu Name w cookie autentykacji.
    /// </summary>
    public string? UserName { get; set; }
}