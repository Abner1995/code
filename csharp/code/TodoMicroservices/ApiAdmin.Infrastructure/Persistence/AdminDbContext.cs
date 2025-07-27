using ApiAdmin.Domain.Entities;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiAdmin.Infrastructure.Persistence;

public class AdminDbContext : EFContext
{
    public DbSet<Admins> Admins { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    public AdminDbContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
