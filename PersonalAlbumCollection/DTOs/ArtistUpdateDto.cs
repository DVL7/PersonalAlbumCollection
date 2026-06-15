namespace PersonalAlbumCollection.DTOs;

using PersonalAlbumCollection.Models.Enums;

/// <summary>
/// DTO z danymi potrzebnymi do aktualizacji istniejącego artysty.
/// Zawiera te same pola co ArtistCreateDto — wszystkie są nadpisywane przy edycji.
/// Używane przez ArtistService.UpdateAsync() i widok Artists.razor.
/// </summary>
public class ArtistUpdateDto
{
    /// <summary>
    /// Nowa nazwa artysty lub zespołu. Wymagana, maksymalnie 50 znaków.
    /// Musi być unikalna w obrębie kolekcji użytkownika.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Typ artysty — SoloArtist lub Band.
    /// </summary>
    public ArtistType ArtistType { get; set; }

    /// <summary>
    /// Kraj pochodzenia artysty (opcjonalny). Maksymalnie 50 znaków.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Opis lub notatka o artyście (opcjonalna). Maksymalnie 2000 znaków.
    /// </summary>
    public string? Description { get; set; }
}