using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        logger.LogInformation("Get");
        return Ok(new Guid());
    }
}
