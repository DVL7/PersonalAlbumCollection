// ICurrentUserService.cs

namespace PersonalAlbumCollection.Services.Interfaces;

/// <summary>
/// Implementacja serwisu użytkownika przechowująca dane zalogowanego użytkownika.
/// Inicjalizowana przy starcie każdego requestu — odczytuje claims z cookie autentykacji.
/// Udostępnia metody Login i Logout do ręcznej zmiany stanu oraz event OnChange
/// do powiadamiania komponentów Blazor o zmianie stanu logowania.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Id zalogowanego użytkownika. Null gdy nikt nie jest zalogowany.
    /// </summary>
    int? UserId { get; }
    /// <summary>
    /// Nazwa zalogowanego użytkownika. Null gdy nikt nie jest zalogowany.
    /// </summary>
    string? UserName { get; }
    /// <summary>
    /// Określa czy użytkownik jest zalogowany.
    /// True gdy UserId ma wartość, false gdy jest null.
    /// </summary>
    bool IsLoggedIn { get; }

    /// <summary>
    /// Event wywoływany po każdej zmianie stanu logowania.
    /// Subskrybowany przez komponenty Blazor do odświeżania UI.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Ustawia dane zalogowanego użytkownika i powiadamia subskrybentów.
    /// Wywoływane po ręcznym zalogowaniu w komponencie Blazor.
    /// </summary>
    /// <param name="userId">Id zalogowanego użytkownika.</param>
    /// <param name="userName">Nazwa zalogowanego użytkownika.</param>
    void Login(int userId, string userName);
    /// <summary>
    /// Czyści dane użytkownika i powiadamia subskrybentów.
    /// Wywoływane po wylogowaniu użytkownika.
    /// </summary>
    void Logout();
}
