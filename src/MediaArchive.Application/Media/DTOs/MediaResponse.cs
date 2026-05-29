namespace MediaArchive.Application.Media.DTOs;

public class MediaResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;
}