using System.Diagnostics;
using System.Text.Json;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;

namespace MediaArchive.Infrastructure.Services;

public class MediaMetadataService : IMediaMetadataService
{
    public async Task<MediaMetadataDto> ExtractAsync(string url)
    {
        var process = new Process();

        process.StartInfo.FileName = "yt-dlp";

        process.StartInfo.Arguments = $"--dump-single-json \"{url}\"";

        process.StartInfo.RedirectStandardOutput = true;

        process.StartInfo.RedirectStandardError = true;

        process.StartInfo.UseShellExecute = false;

        process.Start();

        string json = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        var result = JsonSerializer.Deserialize<YoutubeMetadataResponse>(json);

        if (result == null)
        {
            throw new Exception("Unable to extract metadata");
        }

        return new MediaMetadataDto
        {
            Title = result.Title ?? "",
            DurationSeconds = result.Duration ?? 0,
            ThumbnailUrl = result.Thumbnail ?? "",
            Platform = result.Extractor ?? ""
        };
    }
}