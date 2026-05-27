using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;

namespace PersonalAlbumCollection.Services.Interfaces;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllAsync();
    Task<GenreDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(string name);
    Task<bool> UpdateAsync(int id, string newName);
    Task<bool> DeleteAsync(int id);
    
}