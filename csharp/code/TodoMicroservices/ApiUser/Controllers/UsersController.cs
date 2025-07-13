using ApiUser.Application.User.Commands.Login;
using ApiUser.Application.User.Commands.Refresh;
using ApiUser.Application.User.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
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
