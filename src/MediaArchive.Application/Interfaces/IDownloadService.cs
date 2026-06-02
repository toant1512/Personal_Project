namespace MediaArchive.Application.Interfaces;

public interface IDownloadService
{
    Task DownloadAudioAsync(Guid mediaId);
}