using ApiTodo.Domain.Entities;
using ApiTodo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiTodo.Infrastructure.Configs;

public class TodosConfig : IEntityTypeConfiguration<Todos>
{
    public void Configure(EntityTypeBuilder<Todos> builder)
    {
        builder.ToTable($"T_{nameof(Todos)}".ToLower(), t => t.HasComment("Todos表"));
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(60).IsRequired().HasComment("标题");
        builder.Property(t => t.UserId).HasDefaultValue(0L).HasComment("用户ID");
        builder.Property(t => t.Status)
            .HasDefaultValue(TodoStatus.Pending)
            .HasComment("任务状态: Pending, InProgress, Completed, Cancelled");
        builder.Property(t => t.Priority)
            .HasDefaultValue(TodoPriority.Medium)
            .HasComment("任务优先级: Low, Medium, High");
        builder.Property(t => t.DueDate).HasComment("截至时间");
        builder.Property(t => t.CreatedAt).HasComment("创建时间");
        builder.Property(t => t.UpdatedAt).HasComment("更新时间");
    }
}
