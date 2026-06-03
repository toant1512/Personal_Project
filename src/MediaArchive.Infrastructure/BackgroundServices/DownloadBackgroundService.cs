using MediaArchive.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaArchive.Infrastructure.BackgroundServices;

public class DownloadBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDownloadQueue _queue;

    public DownloadBackgroundService(IServiceProvider serviceProvider, IDownloadQueue queue)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var mediaId = await _queue.DequeueAsync(stoppingToken);

            using var scope = _serviceProvider.CreateScope();

            var downloadService = scope.ServiceProvider
                .GetRequiredService<IDownloadService>();

            try
            {
                await downloadService.DownloadAudioAsync(mediaId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download failed: {ex.Message}");
            }
        }
    }
}