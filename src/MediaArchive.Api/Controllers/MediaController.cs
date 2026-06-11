using MediaArchive.Application.Common.Models;
using MediaArchive.Application.Exceptions;
using MediaArchive.Application.Interfaces;
using MediaArchive.Application.Media.DTOs;
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
    public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
    {
        if (request.Page <= 0)
        {
            throw new BadRequestException("Page must be greater than 0");
        }

        if (request.PageSize <= 0)
        {
            throw new BadRequestException("PageSize must be greater than 0");
        }

        var userId = GetUserId();

        var result = await _mediaService.GetAllAsync(userId, request);

        return Ok(result);
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();

        var media = await _mediaService.GetByIdAsync(id, userId);

        return Ok(media);
    }

    [HttpGet("{id}/file")]
    public async Task<IActionResult> DownloadFile(Guid id)
    {
        var userId = GetUserId();

        var media = await _mediaService.GetEntityByIdAsync(id, userId);

        if (!System.IO.File.Exists(media.FilePath))
        {
            return NotFound("File not found");
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(media.FilePath);

        return File(bytes, "audio/mpeg", Path.GetFileName(media.FilePath));
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.Parse(userId!);
    }
}