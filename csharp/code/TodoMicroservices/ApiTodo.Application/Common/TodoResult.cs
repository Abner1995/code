using ApiTodo.Domain.Enums;

namespace ApiTodo.Application.Common;

public class TodoResult
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long? UserId { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TodoResult()
    {

    }

    public TodoResult(string Title, TodoStatus Status, TodoPriority Priority, long? UserId = 0)
    {
        this.Title = Title;
        this.Status = Status;
        this.Priority = Priority;
        this.UserId = UserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
