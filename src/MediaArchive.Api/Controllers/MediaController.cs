using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;
using MediaArchive.Application.Media.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediaArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly IDownloadService _downloadService;
    private readonly IDownloadQueue _downloadQueue;

    public MediaController(IMediaService mediaService, IDownloadService downloadService, IDownloadQueue downloadQueue)
    {
        _mediaService = mediaService;
        _downloadService = downloadService;
        _downloadQueue = downloadQueue;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMediaRequest request)
    {
        var userId = GetUserId();

        await _mediaService.CreateAsync(userId, request);

        return Ok("Media created");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        var media = await _mediaService.GetAllAsync(userId);

        return Ok(media);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        await _mediaService.DeleteAsync(id, userId);

        return Ok("Media deleted");
    }

    [HttpPost("download/{id}")]
    public async Task<IActionResult> Download(Guid id)
    {
        _downloadQueue.QueueDownload(id);

        await _mediaService.QueueDownloadAsync(id);

        return Accepted("Download queued");
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.Parse(userId!);
    }
}