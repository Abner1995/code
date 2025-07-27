using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ApiAdmin.Application.Admin;

public class CurrentAdmin : ClaimsPrincipal
{
    public CurrentAdmin(IHttpContextAccessor contextAccessor) : base(contextAccessor.HttpContext.User) { }

    public int Id => int.Parse(FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
