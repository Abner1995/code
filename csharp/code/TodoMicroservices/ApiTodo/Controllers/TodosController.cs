using Microsoft.AspNetCore.Mvc;

namespace ApiTodo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController(ILogger<TodosController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        logger.LogInformation("Todo服务");
        return Ok("Todo服务");
    }
}
