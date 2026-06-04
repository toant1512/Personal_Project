namespace MediaArchive.Application.Media.DTOs;

public class MediaResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string FilePath { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;
}