using ApiUser.Domain.Entities;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiUser.Infrastructure.Persistence;

public class UserDbContext : EFContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public UserDbContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options, mediator, capBus)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
