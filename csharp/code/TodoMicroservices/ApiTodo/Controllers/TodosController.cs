using ApiTodo.Application.Todos.Commands.AddTodo;
using ApiTodo.Application.Todos.Commands.DeleteTodo;
using ApiTodo.Application.Todos.Commands.EditTodo;
using ApiTodo.Application.Todos.Queries.GetAllTodo;
using ApiTodo.Application.Todos.Queries.GetTodoById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTodo.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodosController(ILogger<TodosController> logger, IMediator mediator) : ControllerBase
{
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] AddTodoCommand command)
    {
        var response = await mediator.Send(command);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpPost("Edit")]
    public async Task<IActionResult> Edit([FromBody] EditTodoCommand command)
    {
        var response = await mediator.Send(command);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllTodoQuery query)
    {
        var response = await mediator.Send(query);
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetId([FromRoute] int id)
    {
        var response = await mediator.Send(new GetTodoByIdQuery { Id = id });
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await mediator.Send(new DeleteTodoCommand { Id = id });
        return response.Code == 200 ? Ok(response) : BadRequest(response);
    }
}
