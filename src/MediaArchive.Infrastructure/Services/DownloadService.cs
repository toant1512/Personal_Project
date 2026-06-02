using System.Diagnostics;
using MediaArchive.Application.Interfaces;
using MediaArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediaArchive.Infrastructure.Services;

public class DownloadService : IDownloadService
{
    private readonly ApplicationDbContext _context;

    public DownloadService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task DownloadAudioAsync(Guid mediaId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(x => x.Id == mediaId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        try
        {
            media.Status = "Downloading";

            await _context.SaveChangesAsync();

            var solutionRoot = Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "..",
                    ".."));

            var outputPath = Path.Combine(
                    solutionRoot,
                    "downloads",
                    $"{media.Id}.mp3");

            var process = new Process();

            process.StartInfo.FileName = "yt-dlp";

            process.StartInfo.Arguments =
                $"-x --audio-format mp3 " +
                $"-o \"{outputPath}\" " +
                $"\"{media.SourceUrl}\"";

            process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.UseShellExecute = false;

            process.Start();

            await process.WaitForExitAsync();

            media.FilePath = outputPath;

            media.Status = "Completed";

            await _context.SaveChangesAsync();
        }
        catch
        {
            media.Status = "Failed";

            await _context.SaveChangesAsync();

            throw;
        }
    }
}