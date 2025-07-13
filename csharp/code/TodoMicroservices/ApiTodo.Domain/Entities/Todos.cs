using ApiTodo.Domain.Enums;
using Todo.Domain.Abstractions;

namespace ApiTodo.Domain.Entities;

public class Todos : Entity<long>, IAggregateRoot
{
    public string Title { get; set; }
    public long? UserId { get; set; }
    public TodoStatus Status { get; set; }
    public TodoPriority Priority { get; set; }
    public DateTime? DueDate { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Todos()
    {

    }

    public Todos(string title, long userId = 0)
    {
        Title = title;
        UserId = userId;
        Status = TodoStatus.Pending;
        Priority = TodoPriority.Medium;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = TodoStatus.Completed;
        UpdateTimestamp();
    }

    public void Update(string title, TodoStatus status, TodoPriority priority, DateTime? dueDate)
    {
        Title = title;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        UpdateTimestamp();
    }

    private void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
