using MediaArchive.Application.Interfaces;
using MediaArchive.Domain.Enums;
using MediaArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            media.Status = DownloadStatus.Downloading;

            await _context.SaveChangesAsync();

            var solutionRoot = Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "..",
                    ".."));

            var outputPath = Path.Combine(
                    solutionRoot,
                    "downloads",
                    $"{media.Id}.mp3"); // may change to media.Title but need to handle it a bit

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

            media.Status = DownloadStatus.Completed;

            await _context.SaveChangesAsync();
        }
        catch
        {
            media.Status = DownloadStatus.Failed;

            await _context.SaveChangesAsync();

            throw;
        }
    }
}