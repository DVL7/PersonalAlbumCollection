// ApplicationUser.cs

namespace PersonalAlbumCollection.Models.Entities;

/// <summary>
/// Encja użytkownika aplikacji przechowywana w tabeli Users.
/// Zawiera dane logowania oraz kolekcje powiązanych zasobów.
/// </summary>
public class ApplicationUser 
{
    /// <summary>
    /// Klucz glowny encji.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unikalna nazwa użytkownika, służąca również jako login, max 50 znaków.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Unikalny email użytkownika.
    /// Przechowywany w postaci małych liter, max 120 znaków.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash hasła użytkownika, generowany przez PasswordHasher.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Data utworzenia konta użytkownika.
    /// Ustawiana automatycznie przy tworzeniu obiektu.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Kolekcja albumów należących do danego użytkownika.
    /// Właściwość nawigacyjna - wypełniana przez EF Core
    /// Usuwana kaskadowo.
    /// </summary>
    public ICollection<Album> Albums { get; set; } = new List<Album>();
    /// <summary>
    /// Kolekcja artystów należących do danego użytkownika.
    /// Właściwość nawigacyjna - wypełniana przez EF Core
    /// Usuwana kaskadowo.
    /// </summary>
    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    /// <summary>
    /// Kolekcja gatunków muzycznych należących do danego użytkownika.
    /// Właściwość nawigacyjna - wypełniana przez EF Core
    /// Usuwana kaskadowo.
    /// </summary>
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}