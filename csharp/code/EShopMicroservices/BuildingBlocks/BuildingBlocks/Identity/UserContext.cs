using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Identity;

/// <summary>
/// 用户上下文实现，从 HTTP 请求头读取用户信息
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => GetHeaderValue("X-User-Id");

    public string? UserName => GetHeaderValue("X-User-Name");

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

    private string? GetHeaderValue(string headerName)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.Request?.Headers?.TryGetValue(headerName, out var headerValue) == true)
        {
            return headerValue.ToString();
        }

        return null;
    }
}