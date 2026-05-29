using MediaArchive.Application.Authentication.DTOs;
using MediaArchive.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediaArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }
}