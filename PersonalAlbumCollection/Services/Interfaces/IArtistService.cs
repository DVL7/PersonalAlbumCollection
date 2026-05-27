using PersonalAlbumCollection.Models.Enums;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

public interface IArtistService
{
    Task<List<ArtistDto>> GetAllAsync(string? search = null);
    Task<ArtistDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(string name, ArtistType artistType, string? country = null, string? description = null);
    Task<bool> UpdateAsync(int id, string name, ArtistType artistType, string? country = null, string? description = null);
    Task<bool> DeleteAsync(int id);
}
