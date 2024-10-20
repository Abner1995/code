# JWT  

## 关键词  
```shell  
Bearer
```  

## 包
```bash  
Microsoft.IdentityModel.Tokens  
System.IdentityModel.Tokens.Jwt  
Swashbuckle.AspNetCore.Filters  
Microsoft.AspNetCore.Authentication.JwtBearer  
Microsoft.AspNetCore.Http.Abstractions  
```  

## dotnet user-jwts CLI
```shell  
dotnet user-jwts create  
dotnet user-jwts print xxx --show-all    
dotnet user-jwts key     
dotnet user-jwts list    
```  

## 如何使用？  

**appsettings.json**  

```json  
{
  "TokenSettings": {
    "Token": "b28f1a6d8c9eac301db2ef745601948e59c0b5d7a40c29f5d0a2c7b6f3d3e8f5"
  }
}
```  

**TokenService.cs**  

```csharp  
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ContactSMS.WebAPI.Common
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("TokenSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(string id)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               _configuration.GetSection("TokenSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // 刷新令牌有效期 7 天
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 从过期的令牌中获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               _configuration.GetSection("TokenSettings:Token").Value!)),
                ValidateLifetime = false // 这里不验证生命周期
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
```  

**Program.cs**  
```csharp  
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("TokenSettings:Token").Value!))
    };
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
``` 

**LoginController.cs**
```csharp  
using ContactSMS.Commons.Responses;
using ContactSMS.Commons.Util;
using ContactSMS.Domain;
using ContactSMS.Domain.Dto;
using ContactSMS.Domain.Entity;
using ContactSMS.WebAPI.Common;
using ContactSMS.WebAPI.Controllers.Request;
using ContactSMS.WebAPI.Controllers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ContactSMS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public LoginController(ILoginRepository loginRepository, IUserRepository userRepository, IConfiguration configuration, TokenService tokenService)
        {
            this._loginRepository = loginRepository;
            this._userRepository = userRepository;
            this._configuration = configuration;
            this._tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            ServiceResponse<TokenResponse> resp = new ServiceResponse<TokenResponse>();
            if (string.IsNullOrEmpty(loginRequest.UserName) ||
                string.IsNullOrEmpty(loginRequest.PassWord))
            {
                resp.Success = false;
                resp.Message = "用户名或密码不能为空";
                return BadRequest(resp);
            }

            var existingUser = await this._userRepository.GetUserByUserNameAsync(loginRequest.UserName);
            if (existingUser == null)
            {
                resp.Success = false;
                resp.Message = "用户或密码错误";
                return BadRequest(resp);
            }
            bool isVerify = PassWordUtil.VerifyPassword(existingUser.PassWord, loginRequest.PassWord);
            if (!isVerify)
            {
                resp.Success = false;
                resp.Message = "密码错误";
                return BadRequest(resp);
            }
            //string token = CreateToken(existingUser);
            string Id = existingUser.Id.ToString();
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, Id),
                new Claim(ClaimTypes.Name, existingUser.UserName),
            };
            string accessToken = this._tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken(existingUser.UserName);
            var tokenResponse = new TokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            resp.Success = true;
            resp.Message = "获取成功";
            resp.Data = tokenResponse;
            return Ok(resp);
        }
    }
}
```  

**TokenController.cs**  
```csharp  
using ContactSMS.Commons.Responses;
using ContactSMS.WebAPI.Common;
using ContactSMS.WebAPI.Controllers.Request;
using ContactSMS.WebAPI.Controllers.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactSMS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public TokenController(TokenService tokenService)
        {
            this._tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            // 验证刷新令牌
            //var isValid = ValidateRefreshToken(request.RefreshToken, request.UserName); // 使用新的刷新令牌验证逻辑

            //if (!isValid)
            //{
            //    return Unauthorized("Invalid refresh token");
            //}
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                var username = principal.Identity.Name;
                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken(username);
                ServiceResponse<TokenResponse> resp = new ServiceResponse<TokenResponse>();
                var tokenResponse = new TokenResponse()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
                resp.Success = true;
                resp.Message = "获取成功";
                resp.Data = tokenResponse;
                return Ok(resp);
            }
            catch
            {
                return Unauthorized();
            }
        }

        private bool ValidateRefreshToken(string refreshToken, string username)
        {
            // 解析和验证刷新令牌
            try
            {
                // 从刷新令牌中获取声明信息
                var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);

                // 从声明中提取用户名并进行匹配
                var tokenUsername = principal.Identity?.Name;

                // 检查令牌中的用户名是否匹配
                return tokenUsername == username;
            }
            catch (Exception)
            {
                // 如果出现异常，表示令牌无效
                return false;
            }
        }
    }
}
```  