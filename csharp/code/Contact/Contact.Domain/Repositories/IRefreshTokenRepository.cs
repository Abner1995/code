using Contact.Domain.Entities;

namespace Contact.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByUserIdAndDeviceIdAsync(string deviceId, int userId);
    Task<List<RefreshToken>?> GetExpiredByUserIdAsync(int userId);
    Task DeleteAsync(RefreshToken refreshToken);
    Task DeleteRangeAsync(List<RefreshToken> refreshTokens);
    Task CleanUpExpiredRefreshTokens(int userId);
    Task ManageRefreshTokenAsync(User user, string token, string? deviceId);
}
