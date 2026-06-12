using System.ComponentModel.DataAnnotations;

namespace MediaArchive.Application.Common.Models;

public class PagedRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than or equal to 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Page size must be between 1 and 50.")]
    public int PageSize { get; set; } = 10;

    [MaxLength(100, ErrorMessage = "Search must not exceed 100 characters.")]
    public string? Search { get; set; }
}