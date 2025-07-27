using ApiAdmin.Application.Admin.Commands.Login;
using ApiAdmin.Application.Admin.Commands.Refresh;
using ApiAdmin.Application.Admin.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiAdmin.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController(ILogger<AdminsController> logger, IMediator mediator) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var response = await mediator.Send(command);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await mediator.Send(command);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpPost("Refresh")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var response = await mediator.Send(command);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }
}
