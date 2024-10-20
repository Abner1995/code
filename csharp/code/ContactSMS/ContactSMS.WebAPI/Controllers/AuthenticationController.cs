using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace ContactSMS.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public record AuthenticationData(string? UserName, string Password);
    public record UserData(long UserId, string UserName, string Title, string EmployeeId);

    public AuthenticationController(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<string> Authentication([FromBody] AuthenticationData data)
    {
        var user = ValidateCredentials(data);
        if (user is null)
        {
            Unauthorized();
        }
        var token = GenerateToken(user!);
        return Ok(token);
    }

    private string GenerateToken(UserData user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                this._configuration.GetSection("Authentication:SecretKey").Value!));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
        claims.Add(new("title", user.Title));
        claims.Add(new("employeeId", user.EmployeeId));

        var token = new JwtSecurityToken(
            this._configuration.GetSection("Authentication:Issuer").Value!,
            this._configuration.GetSection("Authentication:Audience").Value!,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(1),
            signingCredentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserData? ValidateCredentials(AuthenticationData data)
    {
        //示例
        if (CompareValues(data.UserName, "xuzizheng") &&
            CompareValues(data.Password, "123456"))
        {
            return new UserData(1, data.UserName!, "Business owner", "E001");
        }
        if (CompareValues(data.UserName, "zizheng") &&
            CompareValues(data.Password, "123456"))
        {
            return new UserData(1, data.UserName!, "rider owner", "E002");
        }
        return null;
    }

    private bool CompareValues(string? actual, string expected)
    {
        if (actual is not null)
        {
            if (actual.Equals(expected, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
