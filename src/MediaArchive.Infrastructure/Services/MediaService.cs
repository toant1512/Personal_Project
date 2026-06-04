using MediaArchive.Application.Interfaces;
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
    private readonly IMediaMetadataService _metadataService;

    public MediaService(ApplicationDbContext context, IMediaMetadataService metadataService)
    {
        _context = context;
        _metadataService = metadataService;
    }

    public async Task CreateAsync(Guid userId, CreateMediaRequest request)
    {
        var metadata = await _metadataService.ExtractAsync(request.SourceUrl);

        var media = new MediaItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SourceUrl = request.SourceUrl,
            CreatedAt = DateTime.UtcNow,
            Title = metadata.Title,
            Platform = metadata.Platform,
            ThumbnailUrl = metadata.ThumbnailUrl,
            DurationSeconds = metadata.DurationSeconds
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

    public async Task<MediaResponse> GetByIdAsync(Guid mediaId, Guid userId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(x => x.Id == mediaId && x.UserId == userId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        return new MediaResponse
        {
            Id = media.Id,
            Title = media.Title,
            Platform = media.Platform,
            SourceUrl = media.SourceUrl,
            FilePath = media.FilePath,
            ThumbnailUrl = media.ThumbnailUrl,
            DurationSeconds = media.DurationSeconds,
            CreatedAt = media.CreatedAt,
            Status = media.Status.ToString()
        };
    }

    public async Task<MediaItem> GetEntityByIdAsync(Guid mediaId, Guid userId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(x => x.Id == mediaId && x.UserId == userId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        return media;
    }
}