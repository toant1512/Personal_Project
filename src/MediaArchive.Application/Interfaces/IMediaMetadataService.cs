using MediaArchive.Application.Media.DTOs;

namespace MediaArchive.Application.Interfaces;

public interface IMediaMetadataService
{
    Task<MediaMetadataDto> ExtractAsync(string url);
}