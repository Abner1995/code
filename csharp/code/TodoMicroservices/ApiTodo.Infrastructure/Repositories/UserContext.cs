using ApiTodo.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ApiTodo.Infrastructure.Repositories;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long UserId => long.Parse(
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("Missing user ID in token"));
}
