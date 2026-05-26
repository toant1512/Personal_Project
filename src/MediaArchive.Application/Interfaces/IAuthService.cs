namespace MediaArchive.Application.Authentication.Interfaces;

using MediaArchive.Application.Authentication.DTOs;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);

    Task<string> LoginAsync(LoginRequest request);
}