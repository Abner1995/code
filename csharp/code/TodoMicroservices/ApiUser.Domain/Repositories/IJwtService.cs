using ApiUser.Domain.Entities;
using System.Security.Claims;

namespace ApiUser.Domain.Repositories;

public interface IJwtService
{
    public string GenerateToken(User user);
    public string GenerateRefreshToken();
    public int GetTokenExpiry();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
