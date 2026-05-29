namespace MediaArchive.Application.Media.DTOs;

public class CreateMediaRequest
{
    public string Title { get; set; } = null!;

    public string SourceUrl { get; set; } = null!;

    public string Platform { get; set; } = null!;
}