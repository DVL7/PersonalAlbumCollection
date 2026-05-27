using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResultDto> RegisterAsync(string userName, string email, string password);
    Task<LoginResultDto> LoginAsync(string login, string password);
}