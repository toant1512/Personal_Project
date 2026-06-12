using MediaArchive.Application.Common.Models;
using MediaArchive.Application.Media.DTOs;
using MediaArchive.Domain.Entities;

namespace MediaArchive.Application.Interfaces;

public interface IMediaService
{
    Task CreateAsync(Guid userId, CreateMediaRequest request);

    Task<PagedResponse<MediaResponse>>GetAllAsync(Guid userId, PagedRequest request);

    Task DeleteAsync(Guid mediaId, Guid userId);

    Task RequestDownloadAsync(Guid userId, Guid mediaId);

    Task<MediaResponse> GetByIdAsync(Guid mediaId, Guid userId);

    Task<MediaItem> GetEntityByIdAsync(Guid mediaId, Guid userId);
}