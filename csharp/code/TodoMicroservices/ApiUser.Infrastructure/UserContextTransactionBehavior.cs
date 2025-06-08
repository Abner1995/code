using ApiUser.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Todo.Infrastructure.Core.Behaviors;

namespace ApiUser.Infrastructure;

public class UserContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<UserDbContext, TRequest, TResponse>
{
    public UserContextTransactionBehavior(UserDbContext dbContext, ILogger<UserContextTransactionBehavior<TRequest, TResponse>> logger) : base(dbContext, logger)
    {
    }
}
