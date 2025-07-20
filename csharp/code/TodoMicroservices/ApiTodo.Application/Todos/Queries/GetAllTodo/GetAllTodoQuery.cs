using ApiTodo.Application.Common;
using ApiTodo.Application.Todos.Dtos;
using ApiTodo.Domain.Enums;
using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Queries.GetAllTodo;

public class GetAllTodoQuery : IRequest<ApiResponse<PagedResult<TodoDto>>>
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}
