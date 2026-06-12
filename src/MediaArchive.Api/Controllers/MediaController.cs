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

    public MediaController(IMediaService mediaService, IDownloadService downloadService)
    {
        _mediaService = mediaService;
        _downloadService = downloadService;
    }

    [HttpPost("save")]
    public async Task<IActionResult> Create(CreateMediaRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        await _mediaService.CreateAsync(userId.Value, request);

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
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        var result = await _mediaService.GetAllAsync(userId.Value, request);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        await _mediaService.DeleteAsync(id, userId.Value);

        return Ok("Media deleted");
    }

    [HttpPost("download/{id}")]
    public async Task<IActionResult> Download(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        await _mediaService.RequestDownloadAsync(userId.Value, id);

        return Accepted("Download queued");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        var media = await _mediaService.GetByIdAsync(id, userId.Value);

        return Ok(media);
    }

    [HttpGet("extract-file/{id}")]
    public async Task<IActionResult> ExtractFile(Guid id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized("User unauthorized");
        }

        var media = await _mediaService.GetEntityByIdAsync(id, userId.Value);

        if (!System.IO.File.Exists(media.FilePath))
        {
            return NotFound("File not found");
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(media.FilePath);

        return File(bytes, "audio/mpeg", Path.GetFileName(media.FilePath));
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var result)
        ? result
        : null;
    }
}