using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Commands.DeleteTodo;

public class DeleteTodoCommand : IRequest<ApiResponse<long>>
{
    public long Id { get; init; }
}
