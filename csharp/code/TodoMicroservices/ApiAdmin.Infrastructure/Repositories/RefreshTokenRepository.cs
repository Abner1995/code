using ApiAdmin.Domain.Entities;
using ApiAdmin.Domain.Repositories;
using ApiAdmin.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Infrastructure.Core;

namespace ApiAdmin.Infrastructure.Repositories;

internal class RefreshTokenRepository : Repository<RefreshToken, long, AdminDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AdminDbContext adminDbContext) : base(adminDbContext)
    {

    }

    public async Task<int> CleanUpExpiredRefreshTokens(long adminId, CancellationToken cancellationToken)
    {
        var refreshTokens = await GetExpiredByAdminIdAsync(adminId, cancellationToken);

        if (refreshTokens == null || !refreshTokens.Any())
        {
            return 0;
        }

        try
        {
            DbContext.Set<RefreshToken>().RemoveRange(refreshTokens);
            var affectedRows = await DbContext.SaveChangesAsync(cancellationToken);
            return affectedRows;
        }
        catch (Exception ex)
        {
            throw new Exception("清理过期RefreshToken失败", ex);
        }
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await DbContext.Set<RefreshToken>()
            .Where(r => r.Token.Equals(token))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<RefreshToken>?> GetExpiredByAdminIdAsync(long adminId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == adminId && rt.Expiry < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task ManageRefreshTokenAsync(Admins admin, string token, string? deviceId, CancellationToken cancellationToken)
    {
        if (deviceId != null && admin != null)
        {
            var refresh = await DbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == admin.Id && rt.DeviceId == deviceId)
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
                    UserId = admin.Id,
                    DeviceId = deviceId,
                    Expiry = expiry
                };
                await DbContext.Set<RefreshToken>().AddAsync(refreshData);
            }
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
