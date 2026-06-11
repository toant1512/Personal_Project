using MediaArchive.Application.Interfaces;
using MediaArchive.Domain.Enums;
using System.Threading.Channels;

namespace MediaArchive.Infrastructure.Services;

public class DownloadQueue : IDownloadQueue
{
    private readonly Channel<Guid> _queue;

    public DownloadQueue()
    {
        _queue = Channel.CreateUnbounded<Guid>();
    }

    public void QueueDownload(Guid mediaId)
    {
        _queue.Writer.TryWrite(mediaId);
    }

    public async ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}