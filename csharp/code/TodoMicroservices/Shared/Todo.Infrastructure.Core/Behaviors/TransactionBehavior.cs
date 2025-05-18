using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Infrastructure.Core.Extensions;

namespace Todo.Infrastructure.Core.Behaviors
{
    public class TransactionBehavior<TDbContext, TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : EFContext
    {
        private readonly ILogger<TransactionBehavior<TDbContext, TRequest, TResponse>> _logger;
        private readonly TDbContext _dbContext;

        public TransactionBehavior(
            TDbContext dbContext,
            ILogger<TransactionBehavior<TDbContext, TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var typeName = request.GetGenericTypeName();

            // 如果已有活动事务，直接执行下一个行为
            if (_dbContext.HasActiveTransaction)
            {
                _logger.LogInformation("----- 使用现有事务执行 {CommandName}", typeName);
                return await next();
            }

            // 创建执行策略（支持重试）
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.BeginTransactionAsync();
                using (_logger.BeginScope("TransactionContext:{TransactionId}", transaction.TransactionId))
                {
                    try
                    {
                        _logger.LogInformation(
                            "----- 开始事务 {TransactionId} ({CommandName})",
                            transaction.TransactionId,
                            typeName);

                        // 执行后续中间件和处理器
                        var response = await next();

                        _logger.LogInformation(
                            "----- 提交事务 {TransactionId} ({CommandName})",
                            transaction.TransactionId,
                            typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        return response;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "----- 事务失败 {TransactionId} ({CommandName})",
                            transaction.TransactionId,
                            typeName);

                        // 事务会自动回滚（using 块结束时 Dispose 会触发回滚）
                        throw;
                    }
                }
            });
        }
    }
}