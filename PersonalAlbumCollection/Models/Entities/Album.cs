namespace PersonalAlbumCollection.Models.Entities;

/// <summary>
/// Encja albumu muzycznego przechowywana w bazie danych.
/// Zawiera podstawowe metadane oraz relacje do artystow, gatunkow i uzytkownikow.
/// </summary>
public class Album
{
    /// <summary>
    /// Klucz glowny encji.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tytul albumu. Domyslnie pusty string.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Rok wydania albumu (opcjonalny).
    /// </summary>
    public int? ReleaseYear { get; set; }

    /// <summary>
    /// Adres URL okladki albumu (opcjonalny).
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// Opis lub notatka o albumie (opcjonalna).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Relacje albumu z artystami (tabela laczaca AlbumArtist).
    /// </summary>
    public ICollection<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

    /// <summary>
    /// Relacje albumu z gatunkami (tabela laczaca AlbumGenre).
    /// </summary>
    public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();

    /// <summary>
    /// Relacje albumu z uzytkownikami, np. ocena lub status (tabela UserAlbum).
    /// </summary>
    public ICollection<UserAlbum> UserAlbums { get; set; } = new List<UserAlbum>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
