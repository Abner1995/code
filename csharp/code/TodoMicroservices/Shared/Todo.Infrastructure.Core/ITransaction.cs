using Microsoft.EntityFrameworkCore.Storage;

namespace Todo.Infrastructure.Core;

public interface ITransaction
{
    IDbContextTransaction GetCurrentTransaction();

    bool HasActiveTransaction { get; }

    Task<IDbContextTransaction> BeginTransactionAsync();

    Task CommitTransactionAsync(IDbContextTransaction transaction);

    void RollbackTransaction();
}
