// Artist.cs

using PersonalAlbumCollection.Models.Enums;

namespace PersonalAlbumCollection.Models.Entities;

/// <summary>
/// Encja artysty/zespołu muzycznego przechowywana w tabeli Artists.
/// </summary>
public class Artist
{
    /// <summary>
    /// Klucz glowny encji.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nazwa artysty/zespołu.
    /// Unikalna w obrębie danego użytkownika, max 50 znaków.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Enum składający się z dwóch wartości: 1: SoloArtist, 2: Band.
    /// Wskazuje, czy obiekt jest artystą solowym, czy zespołem.
    /// </summary>
    public ArtistType ArtistType { get; set; }
    
    /// <summary>
    /// Kraj z którym powiązany jest artysta/zespoł. Opcjonalny, max 50 znaków.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Opis lub notatka o artyście/zespołu. Opcjonalny, max 2000 znaków.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Data utworzenia obiektu.
    /// Ustawiana automatycznie przy tworzeniu obiektu.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Klucz obcy do tabeli Users.
    /// Wskazuje który użytkownik jest właścicielem obiektu.
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Podpięty użytkownik, który jest właścicielem albumu. Relacja do encji ApplicationUser.
    /// EF-Core automatycznie wypełnia wartość, w bazie nigdy nie będzie tu wartości null.
    /// Służy do odwołania się do danych użytkownika, np. przy wyświetlaniu danych artysty.
    /// </summary>
    public ApplicationUser User { get; set; }
    
    /// <summary>
    /// Kolekcja powiązań artysty z albumami (tabela łącząca AlbumArtist).
    /// Właściwość nawigacyjna — wypełniana przez EF Core.
    /// </summary>
    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();
    
}