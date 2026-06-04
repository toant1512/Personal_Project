namespace MediaArchive.Application.Media.DTOs;

public class MediaMetadataDto
{
    public string Title { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }
}