using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("用户服务");
        return Ok("用户服务");
    }
}
