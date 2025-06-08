using ApiUser.Application.User.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("User服务");
    }
}
