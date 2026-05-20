namespace MediaArchive.Domain.Entities;

public class MediaItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}