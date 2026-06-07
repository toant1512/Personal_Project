using MediaArchive.Application.Common.Models;
using MediaArchive.Application.Exceptions;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;
using MediaArchive.Application.Media.Interfaces;
using MediaArchive.Domain.Entities;
using MediaArchive.Domain.Enums;
using MediaArchive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaArchive.Infrastructure.Services;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMediaMetadataService _metadataService;
    private readonly ILogger<MediaService> _logger;
    public MediaService(ApplicationDbContext context, IMediaMetadataService metadataService, ILogger<MediaService> logger)
    {
        _context = context;
        _metadataService = metadataService;
        _logger = logger;
    }

    public async Task CreateAsync(Guid userId, CreateMediaRequest request)
    {
        var existingMedia = await _context.MediaItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.SourceUrl == request.SourceUrl);

        if (existingMedia != null)
        {
            _logger.LogWarning("Duplicate media attempted by user {UserId}", userId);

            throw new BadRequestException("Media already exists");
        }

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
            DurationSeconds = metadata.DurationSeconds,
            Uploader = metadata.Uploader,
            ChannelName = metadata.ChannelName
        };

        _context.MediaItems.Add(media);

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} created media {MediaId}", userId, media.Id);
    }

    public async Task<PagedResponse<MediaResponse>> GetAllAsync(Guid userId, PagedRequest request)
    {
        var query = _context.MediaItems
            .Where(x => x.UserId == userId && x.Title.ToLower().Contains(request.Search!.ToLower()));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip(
                (request.Page - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new MediaResponse
            {
                Id = x.Id,
                Title = x.Title,
                Platform = x.Platform,
                SourceUrl = x.SourceUrl,
                FilePath = x.FilePath,
                ThumbnailUrl = x.ThumbnailUrl,
                DurationSeconds = x.DurationSeconds,
                Uploader = x.Uploader,
                ChannelName = x.ChannelName,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString()
            })
            .ToListAsync();

        return new PagedResponse<MediaResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

    public async Task DeleteAsync(Guid mediaId, Guid userId)
    {
        var media = await _context.MediaItems
            .FirstOrDefaultAsync(
                x => x.Id == mediaId &&
                     x.UserId == userId);

        if (media == null)
        {
            throw new NotFoundException("Media not found");
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
            throw new NotFoundException("Media not found");
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
            throw new NotFoundException("Media not found");
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
            throw new NotFoundException("Media not found");
        }

        return media;
    }
}