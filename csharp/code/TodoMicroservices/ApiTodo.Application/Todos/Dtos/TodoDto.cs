using ApiTodo.Domain.Enums;

namespace ApiTodo.Application.Todos.Dtos;

public record TodoDto
{
    public long Id { get; init; }
    public string Title { get; init; }
    public TodoStatus Status { get; init; }
    public TodoPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
