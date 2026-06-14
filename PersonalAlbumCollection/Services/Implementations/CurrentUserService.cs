// CurrentUserService.cs

using System.Security.Claims;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

/// <summary>
/// Implementacja serwisu użytkownika przechowująca dane zalogowanego użytkownika.
/// Inicjalizowana przy starcie każdego requestu — odczytuje claims z cookie autentykacji.
/// Udostępnia metody Login i Logout do ręcznej zmiany stanu oraz event OnChange
/// do powiadamiania komponentów Blazor o zmianie stanu logowania.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    /// <summary>
    /// Konstruktor — odczytuje dane użytkownika z cookie autentykacji przy inicjalizacji.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// Dostarcza dostęp do HttpContext i ClaimsPrincipal aktualnego requestu.
    /// Wstrzykiwany przez DI.
    /// </param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        LoadFromPrincipal(httpContextAccessor.HttpContext?.User);
    }

    /// <summary>
    /// Id zalogowanego użytkownika. Null gdy nikt nie jest zalogowany.
    /// </summary>
    public int? UserId { get; private set; }

    /// <summary>
    /// Nazwa zalogowanego użytkownika. Null gdy nikt nie jest zalogowany.
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// Określa czy użytkownik jest zalogowany.
    /// True gdy UserId ma wartość, false gdy jest null.
    /// </summary>
    public bool IsLoggedIn => UserId.HasValue;

    /// <summary>
    /// Event wywoływany po każdej zmianie stanu logowania.
    /// Subskrybowany przez komponenty Blazor do odświeżania UI.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Ustawia dane zalogowanego użytkownika i powiadamia subskrybentów.
    /// Wywoływane po ręcznym zalogowaniu w komponencie Blazor.
    /// </summary>
    /// <param name="userId">Id zalogowanego użytkownika.</param>
    /// <param name="userName">Nazwa zalogowanego użytkownika.</param>
    public void Login(int userId, string userName)
    {
        UserId = userId;
        UserName = userName;
        NotifyStateChanged();
    }

    /// <summary>
    /// Czyści dane użytkownika i powiadamia subskrybentów.
    /// Wywoływane po wylogowaniu użytkownika.
    /// </summary>
    public void Logout()
    {
        UserId = null;
        UserName = null;
        NotifyStateChanged();
    }

    /// <summary>
    /// Wywołuje event OnChange powiadamiając wszystkich subskrybentów o zmianie stanu.
    /// </summary>
    private void NotifyStateChanged() => OnChange?.Invoke();

    /// <summary>
    /// Odczytuje dane użytkownika z ClaimsPrincipal cookie autentykacji.
    /// Wywoływana w konstruktorze przy każdym nowym requeście.
    /// </summary>
    /// <param name="user">
    /// ClaimsPrincipal reprezentujący tożsamość użytkownika z cookie autentykacji.
    /// Null gdy użytkownik nie jest zalogowany lub brak HttpContext.
    /// </param>
    private void LoadFromPrincipal(ClaimsPrincipal? user)
    {
        // Gdy użytkownik nie jest uwierzytelniony — nic nie rób.
        if (user?.Identity?.IsAuthenticated != true)
            return;

        // Odczyt Id użytkownika z claima NameIdentifier (ustawianego przy logowaniu w Program.cs).
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var userNameClaim = user.Identity?.Name;

        if (int.TryParse(userIdClaim, out var userId))
            UserId = userId;

        if (!string.IsNullOrWhiteSpace(userNameClaim))
            UserName = userNameClaim;
    }
}
