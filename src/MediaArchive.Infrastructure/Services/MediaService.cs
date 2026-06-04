using MediaArchive.Application.Media.DTOs;
using MediaArchive.Application.Media.Interfaces;
using MediaArchive.Domain.Entities;
using MediaArchive.Domain.Enums;
using MediaArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediaArchive.Infrastructure.Services;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _context;

    public MediaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Guid userId, CreateMediaRequest request)
    {
        var media = new MediaItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            SourceUrl = request.SourceUrl,
            Platform = request.Platform,
            CreatedAt = DateTime.UtcNow
        };

        _context.MediaItems.Add(media);

        await _context.SaveChangesAsync();
    }

    public async Task<List<MediaResponse>> GetAllAsync(Guid userId)
    {
        return await _context.MediaItems
            .Where(x => x.UserId == userId)
            .Select(x => new MediaResponse
            {
                Id = x.Id,
                Title = x.Title,
                Platform = x.Platform,
                SourceUrl = x.SourceUrl,
                FilePath = x.FilePath,
                ThumbnailUrl = x.ThumbnailUrl,
                DurationSeconds = x.DurationSeconds,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid mediaId, Guid userId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(
                x => x.Id == mediaId &&
                     x.UserId == userId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        _context.MediaItems.Remove(media);

        await _context.SaveChangesAsync();
    }

    public async Task QueueDownloadAsync(Guid mediaId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(x => x.Id == mediaId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        media.Status = DownloadStatus.Queued;

        await _context.SaveChangesAsync();
    }
}