using System.Text.Json.Serialization;

namespace MediaArchive.Application.Media.DTOs;

public class YoutubeMetadataResponse
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }

    [JsonPropertyName("extractor")]
    public string? Extractor { get; set; }

    [JsonPropertyName("uploader")]
    public string? Uploader { get; set; }

    [JsonPropertyName("channel")]
    public string? Channel { get; set; }
}