using ApiTodo.Domain.Enums;
using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Commands.EditTodo;

public class EditTodoCommand : IRequest<ApiResponse<long>>
{
    public long Id { get; set; }
    public string Title { get; set; }
    //public long? UserId { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
