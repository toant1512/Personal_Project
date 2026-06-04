using System.Text.Json.Serialization;

namespace MediaArchive.Infrastructure.Services;

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
}