using System.ComponentModel.DataAnnotations;

namespace MediaArchive.Application.Media.DTOs;

public class CreateMediaRequest
{
    [Required(ErrorMessage = "Source URL is required.")]
    [Url(ErrorMessage = "Source URL must be a valid URL.")]
    [MaxLength(2048, ErrorMessage = "Source URL must not exceed 2048 characters.")]
    public string SourceUrl { get; set; } = null!;
}