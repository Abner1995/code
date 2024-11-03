using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactSMS.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiVersionNeutral]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    [HttpGet]
    [Route("ping")]
    public IActionResult Ping()
    {
        return Ok("ok");
    }
}
