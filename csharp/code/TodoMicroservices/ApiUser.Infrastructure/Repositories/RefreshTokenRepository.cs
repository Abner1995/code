using ApiUser.Domain.Entities;
using ApiUser.Domain.Repositories;
using ApiUser.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiUser.Infrastructure.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken, long, UserDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(UserDbContext userDbContext) : base(userDbContext)
    {

    }

    public async Task<int> CleanUpExpiredRefreshTokens(long userId, CancellationToken cancellationToken)
    {
        var refreshTokens = await GetExpiredByUserIdAsync(userId, cancellationToken);

        if (refreshTokens == null || !refreshTokens.Any())
        {
            return 0;
        }

        //await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            DbContext.Set<RefreshToken>().RemoveRange(refreshTokens);
            var affectedRows = await DbContext.SaveChangesAsync(cancellationToken);
            //await transaction.CommitAsync(cancellationToken);
            return affectedRows;
        }
        catch (Exception ex)
        {
            //await transaction.RollbackAsync(CancellationToken.None);
            throw new Exception("清理过期RefreshToken失败", ex);
        }
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await DbContext.Set<RefreshToken>()
            .Where(r => r.Token.Equals(token))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<RefreshToken>?> GetExpiredByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.Expiry < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task ManageRefreshTokenAsync(User user, string token, string? deviceId, CancellationToken cancellationToken)
    {
        if (deviceId != null && user != null)
        {
            var refresh = await DbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == user.Id && rt.DeviceId == deviceId)
            .FirstOrDefaultAsync();
            var expiry = DateTime.UtcNow.AddDays(7);
            if (refresh != null)
            {
                refresh.Token = token;
                refresh.Expiry = expiry;
                DbContext.Set<RefreshToken>().Update(refresh);
            }
            else
            {
                var refreshData = new RefreshToken
                {
                    Token = token,
                    UserId = user.Id,
                    DeviceId = deviceId,
                    Expiry = expiry
                };
                await DbContext.Set<RefreshToken>().AddAsync(refreshData);
            }
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
