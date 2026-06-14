// AccountResultDto.cs

namespace PersonalAlbumCollection.DTOs;

/// <summary>
/// Obiekt DTO reprezentujący wynik operacji związanych z CRUDem użytkownika.
/// Klasa dziedziczona przez RegisterResultDto.
/// Używane przez AuthService i RegisterResultDto.
/// </summary>
public class AccountResultDto
{
    /// <summary>
    /// Określa czy operacja zakończyła się powodzeniem.
    /// true = powodzenie, false = niepowodzenie.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Komunikat o błędzie, zwracany gdy Success = false.
    /// Gdy Success = true, ErrorMassage = null.
    /// </summary>
    public string? ErrorMessage { get; set; }
}