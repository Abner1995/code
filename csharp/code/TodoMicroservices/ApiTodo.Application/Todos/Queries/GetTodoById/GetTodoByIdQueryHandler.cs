using ApiTodo.Application.Todos.Dtos;
using ApiTodo.Domain.Repositories;
using MediatR;
using Todo.Core;
using Todo.Core.Exceptions;

namespace ApiTodo.Application.Todos.Queries.GetTodoById;

public class GetTodoByIdQueryHandler(ITodosRepository todosRepository)
    : IRequestHandler<GetTodoByIdQuery, ApiResponse<TodoDto>>
{
    public async Task<ApiResponse<TodoDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await todosRepository.GetAsync(request.Id);
        if (todo is null)
        {
            throw new NotFoundException(nameof(todo), request.Id.ToString());
        }
        var dto = new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Status = todo.Status,
            Priority = todo.Priority,
            DueDate = todo.DueDate,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
        return ApiResponse<TodoDto>.Success(dto);
    }
}
