using Contact.Domain.Entities;
using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Contact.Infrastructure.Repositories;

internal class RefreshTokenRepository(ContactDbContexts dbContexts) : IRefreshTokenRepository
{
    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        await dbContexts.RefreshTokens.AddAsync(refreshToken);
        await dbContexts.SaveChangesAsync();
        return refreshToken;
    }

    public async Task CleanUpExpiredRefreshTokens(int userId)
    {
        var refreshTokens = await GetExpiredByUserIdAsync(userId);
        await DeleteRangeAsync(refreshTokens);
    }

    public async Task DeleteAsync(RefreshToken refreshToken)
    {
        dbContexts.RefreshTokens.Remove(refreshToken);
        await dbContexts.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(List<RefreshToken> refreshTokens)
    {
        dbContexts.RefreshTokens.RemoveRange(refreshTokens);
        await dbContexts.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await dbContexts.RefreshTokens
                .FirstOrDefaultAsync(u => u.Token == token);
    }

    public async Task<RefreshToken?> GetByUserIdAndDeviceIdAsync(string deviceId, int userId)
    {
        return await dbContexts.RefreshTokens
                .FirstOrDefaultAsync(u => u.DeviceId == deviceId && u.UserId == userId);
    }

    public async Task<List<RefreshToken>?> GetExpiredByUserIdAsync(int userId)
    {
        var currentTime = DateTime.UtcNow;
        return await dbContexts.RefreshTokens.Where(x => x.UserId.Equals(userId) && x.Expiry <= currentTime).ToListAsync();
    }

    public async Task ManageRefreshTokenAsync(User user, string token, string? deviceId)
    {
        if (deviceId != null && user != null)
        {
            var refresh = await GetByUserIdAndDeviceIdAsync(deviceId, user.Id);
            var expiry = DateTime.UtcNow.AddDays(7);
            if (refresh != null)
            {
                refresh.Token = token;
                refresh.Expiry = expiry;
                await UpdateAsync(refresh);
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
                await AddAsync(refreshData);
            }
        }
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        dbContexts.RefreshTokens.Update(refreshToken);
        await dbContexts.SaveChangesAsync();
    }
}