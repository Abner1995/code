using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Contact.Application.User;

public class CurrentUser : ClaimsPrincipal
{
    public CurrentUser(IHttpContextAccessor contextAccessor) : base(contextAccessor.HttpContext.User) { }

    public int Id => int.Parse(FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
