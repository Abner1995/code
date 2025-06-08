using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ApiUser.Application.User;

public class CurrentUser : ClaimsPrincipal
{
    public CurrentUser(IHttpContextAccessor contextAccessor) : base(contextAccessor.HttpContext.User) { }

    public int Id => int.Parse(FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
