﻿using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Todo.Infrastructure.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Todo.Infrastructure.Core;

public class EFContext : DbContext, IUnitOfWork, ITransaction
{
    protected IMediator _mediator;
    ICapPublisher _capBus;

    public EFContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus) : base(options)
    {
        _mediator = mediator;
        _capBus = capBus;
    }

    #region ITransaction

    private IDbContextTransaction _currentTransaction;
    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;
        _currentTransaction = Database.BeginTransaction(_capBus, autoCommit: false);
        return Task.FromResult(_currentTransaction);
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
                _capBus.Transaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
                _capBus.Transaction = null;
            }
        }
    }

    #endregion

    #region IUnitOfWork

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await _mediator.DispatchDomainEventsAsync(this);
        return true;
    }

    #endregion
}
