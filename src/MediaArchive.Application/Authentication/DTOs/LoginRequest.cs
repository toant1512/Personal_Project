namespace MediaArchive.Application.Authentication.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}