using ApiTodo.Domain.Entities;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiTodo.Infrastructure.Persistence;

public class TodoDbContext : EFContext
{
    public DbSet<Todos> Todos { get; set; }

    public TodoDbContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
