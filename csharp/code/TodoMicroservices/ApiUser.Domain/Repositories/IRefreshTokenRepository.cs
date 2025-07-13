using ApiUser.Domain.Entities;
using Todo.Infrastructure.Core;

namespace ApiUser.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, long>
{
    Task<List<RefreshToken>?> GetExpiredByUserIdAsync(long userId, CancellationToken cancellationToken);

    Task ManageRefreshTokenAsync(User user, string token, string? deviceId, CancellationToken cancellationToken);

    Task<int> CleanUpExpiredRefreshTokens(long userId, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
}
