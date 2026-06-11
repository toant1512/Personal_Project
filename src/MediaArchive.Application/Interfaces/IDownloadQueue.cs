namespace MediaArchive.Application.Interfaces;

public interface IDownloadQueue
{
    void QueueDownload(Guid mediaId);

    ValueTask<Guid> DequeueAsync(
        CancellationToken cancellationToken);
}