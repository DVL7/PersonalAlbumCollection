// AuthService.cs

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

/// <summary>
/// Serwis obsługujący rejestrację, logowanie oraz zarządzanie danymi konta użytkownika.
/// Używa ASP.NET Core PasswordHasher do bezpiecznego przechowywania haseł.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    // PasswordHasher dostarcza bezpieczne hashowanie i weryfikację haseł.
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="context">Kontekst bazy danych wstrzykiwany przez DI.</param>
    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Rejestruje nowego użytkownika w systemie.
    /// </summary>
    /// <param name="userName">Nazwa użytkownika — unikalna, max 50 znaków.</param>
    /// <param name="email">Adres email — unikalny, max 120 znaków.</param>
    /// <param name="password">Hasło — min 6, max 64 znaki.</param>
    /// <returns>RegisterResultDto z Id nowego użytkownika lub komunikatem błędu.</returns>
    public async Task<RegisterResultDto> RegisterAsync(string userName, string email, string password)
    {
        var normalizedUserName = userName?.Trim() ?? string.Empty;
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;

        // Walidacja danych wejściowych.
        if (string.IsNullOrWhiteSpace(normalizedUserName))
            return FailRegister("Nazwa jest wymagana.");
        if (string.IsNullOrWhiteSpace(normalizedEmail))
            return FailRegister("Email jest wymagany.");
        if (normalizedUserName.Length > 50)
            return FailRegister("Nazwa musi mieć max 50 znaków.");
        if (normalizedEmail.Length > 120)
            return FailRegister("Email musi mieć max 120 znaków.");

        // Walidacja formatu emaila — sprawdza obecność @ i poprawnej domeny.
        if (!IsValidEmail(normalizedEmail))
            return FailRegister("Email jest niepoprawny.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return FailRegister("Hasło musi mieć min 6 znaków.");
        if (password.Length > 64)
            return FailRegister("Hasło musi mieć max 64 znaki.");

        // Sprawdzenie unikalności nazwy użytkownika i emaila.
        var userExists = await _context.Users
            .AnyAsync(u => u.UserName.ToLower() == normalizedUserName.ToLowerInvariant()
                        || u.Email.ToLower() == normalizedEmail);

        if (userExists)
            return FailRegister("Nazwa użytkownika lub Email nie są poprawne.");

        // Utworzenie nowego użytkownika z zahashowanym hasłem.
        var user = new ApplicationUser
        {
            UserName = normalizedUserName,
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterResultDto { Success = true, UserId = user.Id };
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Weryfikuje dane logowania użytkownika.
    /// </summary>
    /// <param name="login">Nazwa użytkownika lub adres email.</param>
    /// <param name="password">Hasło użytkownika.</param>
    /// <returns>
    /// LoginResultDto z Id i nazwą użytkownika gdy logowanie się powiodło,
    /// lub Success = false gdy dane są nieprawidłowe.
    /// </returns>
    public async Task<LoginResultDto> LoginAsync(string login, string password)
    {
        var normalizedLogin = login?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedLogin) || string.IsNullOrWhiteSpace(password))
            return new LoginResultDto { Success = false };

        // Wyszukiwanie użytkownika po nazwie lub emailu.
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == normalizedLogin ||
                u.Email.ToLower() == normalizedLogin);

        if (user is null)
            return new LoginResultDto { Success = false };

        // Weryfikacja hasła przez PasswordHasher.
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            return new LoginResultDto { Success = false };

        return new LoginResultDto
        {
            Success = true,
            UserId = user.Id,
            UserName = user.UserName
        };
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Zmienia nazwę użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="newUserName">Nowa nazwa użytkownika — max 50 znaków.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    public async Task<AccountResultDto> ChangeUserNameAsync(int userId, string newUserName)
    {
        var normalized = newUserName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalized))
            return Fail("Nazwa jest wymagana.");
        if (normalized.Length > 50)
            return Fail("Nazwa użytkownika musi mieć max 50 znaków.");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return Fail("Użytkownik nie istnieje.");

        // Sprawdzenie czy nazwa nie jest już zajęta przez innego użytkownika.
        var taken = await _context.Users
            .AnyAsync(u => u.Id != userId && u.UserName.ToLower() == normalized.ToLower());
        if (taken) return Fail("Nazwa użytkownika zajęta.");

        user.UserName = normalized;
        await _context.SaveChangesAsync();

        return new AccountResultDto { Success = true };
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Zmienia adres email użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="newEmail">Nowy adres email — max 120 znaków.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    public async Task<AccountResultDto> ChangeEmailAsync(int userId, string newEmail)
    {
        var normalized = newEmail?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalized))
            return Fail("Email jest wymagany.");
        if (normalized.Length > 120)
            return Fail("Email musi mieć max 120 znaków.");

        // Walidacja formatu emaila — sprawdza obecność @ i poprawnej domeny.
        if (!IsValidEmail(normalized))
            return Fail("Niepoprawny adres email.");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return Fail("Nie znaleziono użytkownika.");

        // Sprawdzenie czy email nie jest już zajęty przez innego użytkownika.
        var taken = await _context.Users
            .AnyAsync(u => u.Id != userId && u.Email.ToLower() == normalized);
        if (taken) return Fail("Ten email jest już używany.");

        user.Email = normalized;
        await _context.SaveChangesAsync();

        return new AccountResultDto { Success = true };
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Zmienia hasło użytkownika po weryfikacji aktualnego hasła.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="currentPassword">Aktualne hasło — wymagane do weryfikacji.</param>
    /// <param name="newPassword">Nowe hasło — min 6, max 64 znaki.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    public async Task<AccountResultDto> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword))
            return Fail("Wymagane hasło.");
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return Fail("Nowe hasło musi mieć min 6 znaków.");
        if (newPassword.Length > 64)
            return Fail("Nowe hasło musi mieć max 64 znaki..");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return Fail("User not found.");

        // Weryfikacja aktualnego hasła przed zmianą.
        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (verify == PasswordVerificationResult.Failed)
            return Fail("Niepoprawne hasło.");

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
        await _context.SaveChangesAsync();

        return new AccountResultDto { Success = true };
    }

    /// <summary>
    /// Metoda asynchroniczna.
    /// Usuwa konto użytkownika po weryfikacji hasła.
    /// Operacja nieodwracalna — usuwa kaskadowo wszystkie dane użytkownika.
    /// </summary>
    /// <param name="userId">Id użytkownika.</param>
    /// <param name="password">Hasło użytkownika — wymagane do potwierdzenia.</param>
    /// <returns>AccountResultDto z informacją o wyniku operacji.</returns>
    public async Task<AccountResultDto> DeleteAccountAsync(int userId, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return Fail("Hasło wymagane do potwierdzenia.");

        var user = await _context.Users.FindAsync(userId);
        if (user is null) return Fail("Nie znaleziono użytkownika.");

        // Weryfikacja hasła przed usunięciem — operacja nieodwracalna.
        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
            return Fail("Błędne hasło.");

        // Usunięcie kaskadowe — EF Core usunie również albumy, artystów i gatunki użytkownika.
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return new AccountResultDto { Success = true };
    }

    /// <summary>
    /// Tworzy AccountResultDto z błędem dla operacji na koncie.
    /// </summary>
    /// <param name="message">Komunikat błędu.</param>
    private static AccountResultDto Fail(string message)
        => new AccountResultDto { Success = false, ErrorMessage = message };

    /// <summary>
    /// Tworzy RegisterResultDto z błędem dla operacji rejestracji.
    /// </summary>
    /// <param name="message">Komunikat błędu.</param>
    private static RegisterResultDto FailRegister(string message)
        => new RegisterResultDto { Success = false, ErrorMessage = message };

    /// <summary>
    /// Sprawdza czy podany adres email ma poprawny format.
    /// Używa System.Net.Mail.MailAddress do walidacji — wymaga obecności @ i poprawnej domeny z kropką.
    /// </summary>
    /// <param name="email">Adres email do sprawdzenia.</param>
    /// <returns>True gdy format jest poprawny, false w przeciwnym razie.</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}