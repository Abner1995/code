using Contact.Domain.Entities;
using System.Security.Claims;

namespace Contact.Domain.Repositories;

public interface IJwtService
{
    public string GenerateToken(User user);

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
