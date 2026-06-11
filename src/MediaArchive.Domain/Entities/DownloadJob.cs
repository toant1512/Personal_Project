namespace MediaArchive.Domain.Entities;

public class DownloadJob
{
    public Guid Id { get; set; }

    public Guid MediaItemId { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ErrorMessage { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}