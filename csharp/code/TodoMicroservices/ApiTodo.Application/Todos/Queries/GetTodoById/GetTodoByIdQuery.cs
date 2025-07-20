using ApiTodo.Application.Todos.Dtos;
using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQuery : IRequest<ApiResponse<TodoDto>>
{
    public long Id { get; init; }
}
