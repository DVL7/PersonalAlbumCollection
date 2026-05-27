using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.Models.Enums;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

public class UserAlbumService : IUserAlbumService
{
    private readonly AppDbContext _context;

    public UserAlbumService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserAlbumListItemDto>> GetCollectionAsync(
        int userId,
        string? search = null,
        int? genreId = null,
        UserAlbumStatus? status = null)
    {
        var query = _context.UserAlbums
            .AsNoTracking()
            .Include(ua => ua.Album)
                .ThenInclude(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
            .Include(ua => ua.Album)
                .ThenInclude(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre)
            .Where(ua => ua.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();

            query = query.Where(ua =>
                ua.Album.Title.ToLower().Contains(normalizedSearch) ||
                ua.Album.AlbumArtists.Any(aa => aa.Artist.Name.ToLower().Contains(normalizedSearch)));
        }

        if (genreId.HasValue)
        {
            query = query.Where(ua => ua.Album.AlbumGenres.Any(ag => ag.GenreId == genreId.Value));
        }

        if (status.HasValue)
        {
            query = query.Where(ua => ua.Status == status.Value);
        }

        return await query
            .OrderBy(ua => ua.Album.Title)
            .Select(ua => new UserAlbumListItemDto
            {
                AlbumId = ua.AlbumId,
                Title = ua.Album.Title,
                ReleaseYear = ua.Album.ReleaseYear,
                CoverUrl = ua.Album.CoverUrl,
                Artists = string.Join(", ", ua.Album.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)
                    .Select(aa => aa.Artist.Name)),
                Genres = string.Join(", ", ua.Album.AlbumGenres
                    .Select(ag => ag.Genre.Name)
                    .OrderBy(name => name)),
                Score = ua.Score,
                Review = ua.Review,
                Status = ua.Status,
                AddedAt = ua.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserAlbumListItemDto?> GetAsync(int userId, int albumId)
    {
        return await _context.UserAlbums
            .AsNoTracking()
            .Include(ua => ua.Album)
                .ThenInclude(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
            .Include(ua => ua.Album)
                .ThenInclude(a => a.AlbumGenres)
                    .ThenInclude(ag => ag.Genre)
            .Where(ua => ua.UserId == userId && ua.AlbumId == albumId)
            .Select(ua => new UserAlbumListItemDto
            {
                AlbumId = ua.AlbumId,
                Title = ua.Album.Title,
                ReleaseYear = ua.Album.ReleaseYear,
                CoverUrl = ua.Album.CoverUrl,
                Artists = string.Join(", ", ua.Album.AlbumArtists
                    .OrderBy(aa => aa.DisplayOrder)
                    .Select(aa => aa.Artist.Name)),
                Genres = string.Join(", ", ua.Album.AlbumGenres
                    .Select(ag => ag.Genre.Name)
                    .OrderBy(name => name)),
                Score = ua.Score,
                Review = ua.Review,
                Status = ua.Status,
                AddedAt = ua.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AddOrUpdateAsync(int userId, UserAlbumUpsertDto dto)
    {
        if (dto.Score is < 1 or > 10)
        {
            throw new InvalidOperationException("Score must be between 1 and 10.");
        }

        string? normalizedReview = null;

        if (!string.IsNullOrWhiteSpace(dto.Review))
        {
            normalizedReview = dto.Review.Trim();

            if (normalizedReview.Length > 3000)
            {
                throw new InvalidOperationException("Review can have at most 3000 characters.");
            }
        }

        var albumExists = await _context.Albums.AnyAsync(a => a.Id == dto.AlbumId);
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!albumExists || !userExists)
        {
            return false;
        }

        var userAlbum = await _context.UserAlbums
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == dto.AlbumId);

        if (userAlbum is null)
        {
            userAlbum = new UserAlbum
            {
                UserId = userId,
                AlbumId = dto.AlbumId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserAlbums.Add(userAlbum);
        }

        userAlbum.Score = dto.Score;
        userAlbum.Review = normalizedReview;
        userAlbum.Status = dto.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(int userId, int albumId)
    {
        var userAlbum = await _context.UserAlbums
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == albumId);

        if (userAlbum is null)
        {
            return false;
        }

        _context.UserAlbums.Remove(userAlbum);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetScoreAsync(int userId, int albumId, int? score)
    {
        if (score is < 1 or > 10)
        {
            throw new InvalidOperationException("Score must be between 1 and 10.");
        }

        var userAlbum = await _context.UserAlbums
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == albumId);

        if (userAlbum is null)
        {
            return false;
        }

        userAlbum.Score = score;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetStatusAsync(int userId, int albumId, UserAlbumStatus status)
    {
        var userAlbum = await _context.UserAlbums
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AlbumId == albumId);

        if (userAlbum is null)
        {
            return false;
        }

        userAlbum.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}
