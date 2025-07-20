using ApiTodo.Application.Common;
using ApiTodo.Application.Todos.Dtos;
using ApiTodo.Application.Todos.Queries.GetAllTodo;
using ApiTodo.Domain.Repositories;
using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Queries.GetAllTodo;

public class GetAllTodoQueryHandler(ITodosRepository todosRepository)
    : IRequestHandler<GetAllTodoQuery, ApiResponse<PagedResult<TodoDto>>>
{
    async Task<ApiResponse<PagedResult<TodoDto>>> IRequestHandler<GetAllTodoQuery, ApiResponse<PagedResult<TodoDto>>>.Handle(GetAllTodoQuery request, CancellationToken cancellationToken)
    {
        var (todos, totalCount) = await todosRepository.GetAllMatchingAsync(request.SearchPhrase, request.PageSize, request.PageNumber, request.SortBy, request.SortDirection, cancellationToken);

        var dtos = todos.Select(t => new TodoDto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            DueDate = t.DueDate
        }).ToList();
        var result = new PagedResult<TodoDto>(dtos, totalCount, request.PageSize, request.PageNumber);
        return ApiResponse<PagedResult<TodoDto>>.Success(result);
    }
}
