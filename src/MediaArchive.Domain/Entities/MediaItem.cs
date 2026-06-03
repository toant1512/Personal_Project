using MediaArchive.Domain.Enums;

namespace MediaArchive.Domain.Entities;

public class MediaItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string FilePath { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; }

    public DownloadStatus Status { get; set; }
}