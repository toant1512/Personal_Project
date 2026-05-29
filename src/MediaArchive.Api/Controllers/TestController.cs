using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaArchive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Protected route accessed");
    }
}