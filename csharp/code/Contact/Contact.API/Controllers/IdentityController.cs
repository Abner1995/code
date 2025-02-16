using Contact.Application.User.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var res = await mediator.Send(command);
        return Ok(res);
    }
}