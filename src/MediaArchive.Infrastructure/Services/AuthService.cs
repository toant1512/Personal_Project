using BCrypt.Net;
using MediaArchive.Application.Authentication.DTOs;
using MediaArchive.Application.Authentication.Interfaces;
using MediaArchive.Domain.Entities;
using MediaArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediaArchive.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == request.Email);

        if (existingUser != null)
        {
            throw new Exception(
                "Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(
        LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == request.Email);

        if (user == null)
        {
            throw new Exception(
                "Invalid credentials");
        }

        bool validPassword =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            throw new Exception(
                "Invalid credentials");
        }

        return "Login successful";
    }
}