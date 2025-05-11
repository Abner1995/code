using Microsoft.AspNetCore.Mvc;

namespace ApiAdmin.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController(ILogger<AdminsController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("管理服务");
        return Ok("管理服务");
    }
}
