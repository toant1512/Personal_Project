using System.ComponentModel.DataAnnotations;

namespace MediaArchive.Application.Authentication.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}