using ApiAdmin.Domain.Entities;
using Todo.Infrastructure.Core;

namespace ApiAdmin.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, long>
{
    Task<List<RefreshToken>?> GetExpiredByAdminIdAsync(long adminId, CancellationToken cancellationToken);

    Task ManageRefreshTokenAsync(Admins admin, string token, string? deviceId, CancellationToken cancellationToken);

    Task<int> CleanUpExpiredRefreshTokens(long adminId, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
}
