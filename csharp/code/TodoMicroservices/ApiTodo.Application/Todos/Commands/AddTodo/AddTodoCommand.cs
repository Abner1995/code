using ApiTodo.Domain.Enums;
using MediatR;
using Todo.Core;

namespace ApiTodo.Application.Todos.Commands.AddTodo;

public class AddTodoCommand : IRequest<ApiResponse<long>>
{
    public string Title { get; set; }
    //public long? UserId { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
