// IAuthService.cs

using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

/// <summary>
/// Serwis obsługujący rejestrację, logowanie oraz zarządzanie danymi konta użytkownika.
/// Używa ASP.NET Core PasswordHasher do bezpiecznego przechowywania haseł.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Rejestruje nowego użytkownika w systemie.
    /// </summary>
    /// <param name="userName">Nazwa użytkownika — unikalna, max 50 znaków.</param>
    /// <param name="email">Adres email — unikalny, max 120 znaków.</param>
    /// <param name="password">Hasło — min 6, max 64 znaki.</param>
    /// <returns>RegisterResultDto z Id nowego użytkownika lub komunikatem błędu.</returns>
    Task<RegisterResultDto> RegisterAsync(string userName, string email, string password);
    /// <summary>
    /// Weryfikuje dane logowania użytkownika.
    /// </summary>
    /// <param name="login">Nazwa użytkownika lub adres email.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <returns>
    /// LoginResultDto z Id i nazwą użytkownika gdy logowanie się powiodło,
    /// lub Success = false gdy dane są nieprawidłowe.
    /// </returns>
    Task<LoginResultDto> LoginAsync(string login, string password);
    /// <summary>
    /// Zmienia nazwę użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="newUserName">Nowa nazwa użytkownika — max 50 znaków.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    Task<AccountResultDto> ChangeUserNameAsync(int userId, string newUserName);
    /// <summary>
    /// Zmienia adres email użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="newEmail">Nowy adres email — max 120 znaków.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    Task<AccountResultDto> ChangeEmailAsync(int userId, string newEmail);
    /// <summary>
    /// Zmienia hasło użytkownika po weryfikacji aktualnego hasła.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="currentPassword">Aktualne hasło — wymagane do weryfikacji.</param>
    /// <param name="newPassword">Nowe hasło — min 6, max 64 znaki.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    Task<AccountResultDto> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    /// <summary>
    /// Usuwa konto użytkownika po weryfikacji hasła.
    /// Operacja nieodwracalna — usuwa kaskadowo wszystkie dane użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="password">Hasło użytkownika — wymagane do potwierdzenia.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    Task<AccountResultDto> DeleteAccountAsync(int userId, string password);
}