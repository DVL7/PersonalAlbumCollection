using PersonalAlbumCollection.Models.Enums;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

public interface IUserAlbumService
{
    Task<List<UserAlbumListItemDto>> GetCollectionAsync(int userId, string? search = null, int? genreId = null, UserAlbumStatus? status = null);
    Task<UserAlbumListItemDto?> GetAsync(int userId, int albumId);
    Task<bool> AddOrUpdateAsync(int userId, UserAlbumUpsertDto dto);
    Task<bool> RemoveAsync(int userId, int albumId);
    Task<bool> SetScoreAsync(int userId, int albumId, int? score);
    Task<bool> SetStatusAsync(int userId, int albumId, UserAlbumStatus status);
}