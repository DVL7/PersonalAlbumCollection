using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

public interface IAlbumService
{
    Task<List<AlbumListItemDto>> GetAllAsync(string? search = null, int? genreId = null);
    Task<AlbumDetailsDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(AlbumCreateDto dto);
    Task<bool> UpdateAsync(int id, AlbumUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}