using ApiAdmin.Domain.Entities;
using System.Security.Claims;

namespace ApiAdmin.Domain.Repositories;

public interface IJwtService
{
    public string GenerateToken(Admins admin);
    public string GenerateRefreshToken();
    public int GetTokenExpiry();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
