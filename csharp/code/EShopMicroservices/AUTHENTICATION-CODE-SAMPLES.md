# 身份认证代码示例

本文档提供身份认证实现的关键代码示例，作为README.md指南的补充。

## 1. Identity.API关键代码

### 1.1 IdentityContext.cs
```csharp
using Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data;

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // 自定义配置
    }
}
```

### 1.2 ApplicationUser.cs
```csharp
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}
```

### 1.3 JwtSettings.cs
```csharp
namespace Identity.API.Models;

public class JwtSettings
{
    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int ExpiryMinutes { get; set; }
}
```

### 1.4 RegisterRequest.cs
```csharp
namespace Identity.API.Models;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword);
```

### 1.5 LoginRequest.cs
```csharp
namespace Identity.API.Models;

public record LoginRequest(string Email, string Password);
```

### 1.6 AuthResponse.cs
```csharp
namespace Identity.API.Models;

public class AuthResponse
{
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
}
```

### 1.7 JWT配置 (Program.cs)
```csharp
// 添加JWT认证
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

// 配置JWT设置
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
```

### 1.4 AuthController.cs (部分)
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        // 生成JWT令牌
        var token = await GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "无效的邮箱或密码" });
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var token = await GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName
        });
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim("userId", user.Id),
            new Claim("userName", user.UserName)
        };

        // 添加用户角色
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## 2. YarpApiGateway认证中间件

### 2.1 Program.cs (网关)
```csharp
// 添加JWT认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["JwtSettings:Issuer"];
        options.Audience = builder.Configuration["JwtSettings:Audience"];
        options.RequireHttpsMetadata = false; // 开发环境设为false
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// 添加用户信息提取中间件
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        // 从JWT令牌中提取用户信息
        var userId = context.User.FindFirstValue("userId");
        var userName = context.User.FindFirstValue("userName");

        if (!string.IsNullOrEmpty(userId))
        {
            // 添加到请求头，传递给后端服务
            context.Request.Headers["X-User-Id"] = userId;
        }
        if (!string.IsNullOrEmpty(userName))
        {
            context.Request.Headers["X-User-Name"] = userName;
        }
    }

    await next();
});
```

### 2.2 appsettings.json (网关路由配置)
```json
{
  "ReverseProxy": {
    "Routes": {
      "identity-route": {
        "ClusterId": "identity-cluster",
        "Match": {
          "Path": "/identity-service/{**catch-all}"
        },
        "Transforms": [ { "PathPattern": "{**catch-all}" } ]
      },
      "basket-route": {
        "ClusterId": "basket-cluster",
        "AuthorizationPolicy": "require-auth", // 需要认证
        "Match": {
          "Path": "/basket-service/{**catch-all}"
        },
        "Transforms": [ { "PathPattern": "{**catch-all}" } ]
      },
      "ordering-route": {
        "ClusterId": "ordering-cluster",
        "AuthorizationPolicy": "require-auth", // 需要认证
        "RateLimiterPolicy": "fixed",
        "Match": {
          "Path": "/ordering-service/{**catch-all}"
        },
        "Transforms": [ { "PathPattern": "{**catch-all}" } ]
      },
      "catalog-public-route": {
        "ClusterId": "catalog-cluster",
        "Match": {
          "Path": "/catalog-service/products/{**catch-all}"
        },
        "Transforms": [ { "PathPattern": "{**catch-all}" } ]
      },
      "catalog-protected-route": {
        "ClusterId": "catalog-cluster",
        "AuthorizationPolicy": "require-auth", // 需要认证
        "Match": {
          "Path": "/catalog-service/admin/{**catch-all}"
        },
        "Transforms": [ { "PathPattern": "{**catch-all}" } ]
      }
    },
    "Clusters": {
      "identity-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://identity.api:8080"
          }
        }
      }
    }
  }
}
```

## 3. Basket.API用户上下文处理

### 3.1 修改GetBasketEndpoints.cs
```csharp
public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket", async (HttpContext context, ISender sender) =>
        {
            // 从请求头获取用户名（由网关添加）
            var userName = context.Request.Headers["X-User-Name"].ToString();

            if (string.IsNullOrEmpty(userName))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetBasketQuery(userName));
            var response = result.Adapt<GetBasketResponse>();
            return Results.Ok(response);
        })
        .RequireAuthorization() // 需要认证
        .WithName("GetBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithSummary("Get current user's basket")
        .WithDescription("Get the shopping basket for the authenticated user");
    }
}
```

### 3.2 添加用户上下文服务
```csharp
// UserContext.cs
public interface IUserContext
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].ToString();

    public string? UserName => _httpContextAccessor.HttpContext?.Request.Headers["X-User-Name"].ToString();

    public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);
}

// 在Program.cs中注册
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
```

### 3.3 修改GetBasketHandler使用用户上下文
```csharp
public class GetBasketHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUserContext _userContext;

    public GetBasketHandler(IBasketRepository basketRepository, IUserContext userContext)
    {
        _basketRepository = basketRepository;
        _userContext = userContext;
    }

    public async Task<GetBasketResult> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        // 从用户上下文获取用户名，而不是从查询参数
        var userName = _userContext.UserName;

        if (string.IsNullOrEmpty(userName))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var basket = await _basketRepository.GetBasket(userName, cancellationToken);
        return new GetBasketResult(basket);
    }
}
```

## 4. Shopping.Web前端认证

### 4.1 AuthService.cs
```csharp
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginModel model);
    Task<AuthResponse> RegisterAsync(RegisterModel model);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetUserNameAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponse> LoginAsync(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["ApiSettings:GatewayAddress"]}/identity-service/api/auth/login",
            new { email = model.Email, password = model.Password });

        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        // 存储token（可以使用localStorage、sessionStorage或HttpOnly Cookie）
        if (authResponse?.Token != null)
        {
            await StoreTokenAsync(authResponse.Token);
        }

        return authResponse!;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterModel model)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_configuration["ApiSettings:GatewayAddress"]}/identity-service/api/auth/register",
            new {
                email = model.Email,
                password = model.Password,
                confirmPassword = model.ConfirmPassword,
                firstName = model.FirstName,
                lastName = model.LastName
            });

        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (authResponse?.Token != null)
        {
            await StoreTokenAsync(authResponse.Token);
        }

        return authResponse!;
    }

    public async Task LogoutAsync()
    {
        await RemoveTokenAsync();
    }

    public async Task<string?> GetTokenAsync()
    {
        // 从存储中获取token
        return await RetrieveTokenAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetUserNameAsync()
    {
        // 可以从token中解析或调用API获取
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return null;

        // 简单示例：实际实现应该解析JWT或调用用户信息API
        return "user"; // 临时返回
    }

    private async Task StoreTokenAsync(string token)
    {
        // 实现token存储逻辑
        // 注意：生产环境应考虑安全性（HttpOnly Cookie、安全存储等）
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Session.SetString("AuthToken", token);
        }
    }

    private async Task<string?> RetrieveTokenAsync()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            return _httpContextAccessor.HttpContext.Session.GetString("AuthToken");
        }
        return null;
    }

    private async Task RemoveTokenAsync()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            _httpContextAccessor.HttpContext.Session.Remove("AuthToken");
        }
    }
}
```

### 4.2 更新IBasketService.cs
```csharp
public interface IBasketService
{
    [Get("/basket-service/basket")]
    [Headers("Authorization: Bearer")]
    Task<GetBasketResponse> GetBasket();

    [Post("/basket-service/basket")]
    [Headers("Authorization: Bearer")]
    Task<StoreBasketResponse> StoreBasket(StoreBasketRequest request);

    [Delete("/basket-service/basket")]
    [Headers("Authorization: Bearer")]
    Task<DeleteBasketResponse> DeleteBasket();

    public async Task<ShoppingCartModel> LoadUserBasket()
    {
        ShoppingCartModel basket;

        try
        {
            // 不再硬编码用户名，使用认证用户
            var getBasketResponse = await GetBasket();
            basket = getBasketResponse.Cart;
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            // 获取当前用户名
            var userName = await GetCurrentUserNameAsync();
            basket = new ShoppingCartModel
            {
                UserName = userName ?? "guest",
                Items = []
            };
        }

        return basket;
    }

    private async Task<string?> GetCurrentUserNameAsync()
    {
        // 从认证服务获取当前用户名
        // 实际实现应该从IUserContext或类似服务获取
        return "authenticated_user";
    }
}
```

### 4.3 Login.cshtml.cs
```csharp
public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _authService.LoginAsync(new LoginModel
            {
                Email = Input.Email,
                Password = Input.Password
            });

            // 登录成功，重定向
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "登录失败: " + ex.Message);
            return Page();
        }
    }
}
```

## 5. Docker Compose配置

### 5.1 docker-compose.yml (添加部分)
```yaml
services:
  identitydb:
    image: postgres

  identity.api:
    image: ${DOCKER_REGISTRY-}identityapi
    build:
      context: .
      dockerfile: Services/Identity/Identity.API/Dockerfile
```

### 5.2 docker-compose.override.yml (添加部分)
```yaml
services:
  identitydb:
    container_name: identitydb
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=IdentityDb
    restart: always
    ports:
        - "5434:5432"
    volumes:
      - postgres_identity:/var/lib/postgresql/data/

  identity.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
      - ConnectionStrings__IdentityDatabase=Server=identitydb;Port=5432;Database=IdentityDb;User Id=postgres;Password=postgres;Include Error Detail=true
      - JwtSettings__Secret=YourSuperSecretKeyHereAtLeast32CharactersLong!
      - JwtSettings__Issuer=EShopIdentity
      - JwtSettings__Audience=EShopServices
      - JwtSettings__ExpiryMinutes=60
    depends_on:
      - identitydb
    ports:
      - "6006:8080"

  yarpapigateway:
    environment:
      # 添加JWT配置
      - JwtSettings__Secret=YourSuperSecretKeyHereAtLeast32CharactersLong!
      - JwtSettings__Issuer=EShopIdentity
      - JwtSettings__Audience=EShopServices
    depends_on:
      # 添加对identity.api的依赖
      - identity.api

  shopping.web:
    environment:
      # 确保前端知道网关地址
      - ApiSettings__GatewayAddress=http://yarpapigateway:8080

volumes:
  postgres_identity:
```

## 6. 环境变量和配置

### 6.1 appsettings.Development.json (Identity.API)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "IdentityDatabase": "Server=localhost;Port=5434;Database=IdentityDb;User Id=postgres;Password=postgres;"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyHereAtLeast32CharactersLong!",
    "Issuer": "EShopIdentity",
    "Audience": "EShopServices",
    "ExpiryMinutes": 60
  }
}
```

## 注意事项

1. **安全性**：
   - 生产环境必须使用强密码和安全的JWT密钥
   - 考虑使用密钥管理系统（如Azure Key Vault、AWS KMS）
   - 启用HTTPS
   - 实现适当的CORS策略

2. **性能**：
   - JWT验证在网关集中处理，避免每个服务重复验证
   - 考虑使用分布式缓存存储吊销的令牌（如果需要令牌吊销）

3. **可扩展性**：
   - Identity.API可以水平扩展，共享同一个数据库
   - 考虑使用Redis存储刷新令牌

4. **测试**：
   - 编写单元测试验证认证逻辑
   - 编写集成测试验证端到端流程
   - 使用Postman或Swagger测试API

这些代码示例提供了实现身份认证的关键部分。根据你的具体需求进行调整和扩展。记住，安全是一个持续的过程，需要不断评估和改进。