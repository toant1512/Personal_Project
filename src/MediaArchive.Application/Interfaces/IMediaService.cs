using MediaArchive.Application.Media.DTOs;

namespace MediaArchive.Application.Media.Interfaces;

public interface IMediaService
{
    Task CreateAsync(Guid userId, CreateMediaRequest request);

    Task<List<MediaResponse>> GetAllAsync(Guid userId);

    Task DeleteAsync(Guid mediaId, Guid userId);
}