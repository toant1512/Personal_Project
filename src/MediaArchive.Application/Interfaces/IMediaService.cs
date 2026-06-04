using MediaArchive.Application.Media.DTOs;
using MediaArchive.Domain.Entities;

namespace MediaArchive.Application.Media.Interfaces;

public interface IMediaService
{
    Task CreateAsync(Guid userId, CreateMediaRequest request);

    Task<List<MediaResponse>> GetAllAsync(Guid userId);

    Task DeleteAsync(Guid mediaId, Guid userId);

    Task QueueDownloadAsync(Guid mediaId);

    Task<MediaResponse> GetByIdAsync(Guid mediaId, Guid userId);

    Task<MediaItem> GetEntityByIdAsync(Guid mediaId, Guid userId);
}