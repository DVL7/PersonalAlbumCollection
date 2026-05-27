using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalAlbumCollection.Data;
using PersonalAlbumCollection.Models.Entities;
using PersonalAlbumCollection.DTOs;
using PersonalAlbumCollection.Services.Interfaces;

namespace PersonalAlbumCollection.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterResultDto> RegisterAsync(string userName, string email, string password)
    {
        var normalizedUserName = userName.Trim();
        var normalizedEmail = email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(normalizedUserName))
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "User name is required."
            };
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Email is required."
            };
        }

        if (normalizedUserName.Length > 50)
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "User name must have at most 50 characters."
            };
        }

        if (normalizedEmail.Length > 120)
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Email must have at most 120 characters."
            };
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "Password must have at least 6 characters."
            };
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.UserName.ToLower() == normalizedUserName.ToLower() || u.Email.ToLower() == normalizedEmail);

        if (userExists)
        {
            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = "User name or email already exists."
            };
        }

        var user = new ApplicationUser
        {
            UserName = normalizedUserName,
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterResultDto
        {
            Success = true,
            UserId = user.Id
        };
    }

    public async Task<LoginResultDto> LoginAsync(string login, string password)
    {
        var normalizedLogin = login.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(normalizedLogin) || string.IsNullOrWhiteSpace(password))
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Invalid login or password."
            };
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == normalizedLogin ||
                u.Email.ToLower() == normalizedLogin);

        if (user is null)
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Invalid login or password."
            };
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Invalid login or password."
            };
        }

        return new LoginResultDto
        {
            Success = true,
            UserId = user.Id,
            UserName = user.UserName
        };
    }
}
