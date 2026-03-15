# EShop微服务系统 - 身份认证功能添加指南

## 项目概述

本指南将带领你为EShop微服务系统添加完整的身份认证功能。作为微服务初学者，通过本实践你将学习到：

1. 如何在微服务架构中设计身份认证系统
2. 如何实现JWT令牌认证
3. 如何在API网关集中处理认证
4. 如何在前端集成登录功能
5. 如何在微服务间传递用户上下文

## 系统当前架构

当前系统包含以下服务：
- **Catalog.API** - 产品目录服务（PostgreSQL + Marten）
- **Basket.API** - 购物车服务（PostgreSQL + Redis + gRPC）
- **Discount.Grpc** - 折扣服务（SQLite + gRPC）
- **Ordering.API** - 订单服务（SQL Server + RabbitMQ）
- **YarpApiGateway** - API网关（YARP反向代理）
- **Shopping.Web** - 前端界面（Razor Pages）

## 认证方案设计

我们将采用以下方案：
1. **集中式认证**：在API网关处理所有认证
2. **JWT令牌**：使用JSON Web Tokens进行无状态认证
3. **ASP.NET Core Identity**：管理用户和角色
4. **Bearer Token**：通过HTTP头部传递令牌

### 架构变化
```
客户端 → [YarpApiGateway + JWT认证] → 后端微服务
                    ↓
              [Identity.API] ←→ [IdentityDb]
```

## 分步实现指南

### 第一阶段：创建Identity.API服务

#### 步骤1.1：创建Identity.API项目
在`Services`目录下创建新的`Identity`文件夹，然后创建`Identity.API`项目。

```bash
# 在Services目录下执行
dotnet new webapi -n Identity.API
```

#### 步骤1.2：添加必要的NuGet包
编辑`Identity.API.csproj`文件，添加以下包引用：

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="8.0.0" />
<PackageReference Include="AspNetCore.HealthChecks.UI.Client" Version="8.0.0" />
<PackageReference Include="BuildingBlocks" Version="1.0.0" />
```

#### 步骤1.3：创建数据库上下文和用户模型
在`Identity.API`项目中创建以下文件：

1. `Data/AppDbContext.cs` - 数据库上下文
2. `Models/ApplicationUser.cs` - 扩展用户模型
3. `Models/RegisterRequest.cs` - 注册请求DTO
4. `Models/LoginRequest.cs` - 登录请求DTO
5. `Models/AuthResponse.cs` - 认证响应DTO

#### 步骤1.4：配置JWT认证
在`Program.cs`中配置：
- ASP.NET Core Identity
- Entity Framework Core + PostgreSQL
- JWT认证方案
- Swagger/OpenAPI支持（可选）

#### 步骤1.5：实现认证控制器
创建`Controllers/AuthController.cs`，实现以下端点：
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/refresh` - 刷新令牌
- `GET /api/auth/user` - 获取当前用户信息

#### 步骤1.6：添加健康检查
在`Program.cs`中添加PostgreSQL健康检查。

### 第二阶段：更新API网关

#### 步骤2.1：在YarpApiGateway中添加JWT认证
在`YarpApiGateway`项目中添加JWT认证包：

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

#### 步骤2.2：配置认证中间件
更新`Program.cs`：
1. 添加JWT认证服务
2. 配置JWT验证参数（与Identity.API一致）
3. 添加认证中间件

#### 步骤2.3：保护路由
更新`appsettings.json`中的YARP路由配置，为需要认证的路由添加认证要求：

```json
{
  "Routes": {
    "ordering-route": {
      "ClusterId": "ordering-cluster",
      "AuthorizationPolicy": "require-auth",  // 添加这行
      "Match": {
        "Path": "/ordering-service/{**catch-all}"
      }
    }
  }
}
```

#### 步骤2.4：传递用户信息
在网关中添加中间件，从JWT令牌中提取用户信息（如用户名、用户ID），并添加到请求头中传递给后端服务。

### 第三阶段：更新现有微服务

#### 步骤3.1：为Basket.API添加用户上下文
Basket.API当前使用`UserName`标识用户。我们需要：
1. 添加从请求头中提取用户信息的中间件
2. 更新`ShoppingCart`模型和相关逻辑使用认证用户

#### 步骤3.2：为Ordering.API添加用户上下文
Ordering.API需要将用户与订单关联：
1. 在`Order`实体中添加`UserId`和`UserName`字段
2. 更新订单创建逻辑使用认证用户
3. 添加查询用户订单的端点

#### 步骤3.3：为Catalog.API添加用户上下文（可选）
如果Catalog.API需要用户特定功能（如收藏、评论），添加用户上下文支持。

#### 步骤3.4：更新所有服务的Dockerfile
确保所有服务都能正确接收和处理用户信息请求头。

### 第四阶段：更新前端应用

#### 步骤4.1：创建认证服务
在`Shopping.Web`项目中创建：
1. `Services/IAuthService.cs` - 认证服务接口
2. `Services/AuthService.cs` - 认证服务实现
3. `Models/LoginModel.cs` - 登录模型
4. `Models/RegisterModel.cs` - 注册模型

#### 步骤4.2：创建登录和注册页面
1. `Pages/Login.cshtml`和`Login.cshtml.cs` - 登录页面
2. `Pages/Register.cshtml`和`Register.cshtml.cs` - 注册页面
3. `Pages/Logout.cshtml` - 登出页面

#### 步骤4.3：添加布局更新
更新`_Layout.cshtml`：
1. 添加登录状态显示
2. 添加登录/登出链接
3. 添加用户欢迎信息

#### 步骤4.4：更新现有页面
更新购物车、结账、订单列表等页面：
1. 检查用户是否已登录
2. 将JWT令牌添加到API请求头中

#### 步骤4.5：添加令牌管理
实现令牌的存储（localStorage或sessionStorage）、刷新和自动续期逻辑。

### 第五阶段：更新Docker Compose配置

#### 步骤5.1：添加Identity数据库
在`docker-compose.yml`中添加新的PostgreSQL容器：

```yaml
identitydb:
  image: postgres
```

#### 步骤5.2：添加Identity.API服务
在`docker-compose.yml`中添加Identity.API服务：

```yaml
identity.api:
  image: ${DOCKER_REGISTRY-}identityapi
  build:
    context: .
    dockerfile: Services/Identity/Identity.API/Dockerfile
```

#### 步骤5.3：更新环境变量
在`docker-compose.override.yml`中配置：
1. Identity.API的连接字符串
2. JWT密钥和其他配置
3. 服务依赖关系

#### 步骤5.4：更新YarpApiGateway配置
在YARP网关的`appsettings.json`中添加Identity.API的路由：

```json
{
  "Routes": {
    "identity-route": {
      "ClusterId": "identity-cluster",
      "Match": {
        "Path": "/identity-service/{**catch-all}"
      }
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
```

### 第六阶段：测试和验证

#### 步骤6.1：启动所有服务
```bash
docker-compose up -d
```

#### 步骤6.2：测试注册和登录
1. 访问 http://localhost:6005/register 注册新用户
2. 访问 http://localhost:6005/login 登录用户
3. 验证JWT令牌是否正确生成

#### 步骤6.3：测试受保护端点
1. 未登录时访问订单页面，应重定向到登录页
2. 登录后访问订单页面，应正常显示
3. 测试购物车功能与用户的关联

#### 步骤6.4：测试网关认证
1. 直接调用需要认证的API端点（不带token），应返回401
2. 使用有效token调用API，应返回正常响应

#### 步骤6.5：集成测试
测试完整的用户流程：
1. 注册 → 登录 → 浏览商品 → 添加到购物车 → 结账 → 查看订单

## 关键技术点说明

### JWT令牌结构
```json
{
  "sub": "用户ID",
  "name": "用户名",
  "email": "用户邮箱",
  "roles": ["用户角色"],
  "exp": 过期时间戳,
  "iss": "令牌发行者",
  "aud": "令牌受众"
}
```

### 网关认证流程
1. 客户端请求携带Bearer Token
2. 网关验证Token签名和有效性
3. 网关提取用户信息并添加到请求头
4. 后端服务从请求头读取用户信息
5. 后端服务执行业务逻辑

### 前端令牌管理策略
1. **存储**：将access token和refresh token存储在HttpOnly Cookie中
2. **刷新**：access token过期时使用refresh token获取新token
3. **安全性**：避免将token存储在localStorage（易受XSS攻击）

## 常见问题及解决方案

### 问题1：跨服务用户上下文传递
**解决方案**：在网关中添加中间件，将用户信息（如`X-User-Id`, `X-User-Name`）添加到请求头中，所有后端服务从这些头部读取用户信息。

### 问题2：令牌刷新机制
**解决方案**：实现双令牌机制（access token + refresh token）。access token短期有效（15-30分钟），refresh token长期有效（7天），用于获取新的access token。

### 问题3：微服务间认证
**解决方案**：服务间通信使用机器对机器（M2M）令牌或直接信任网关传递的用户信息。对于gRPC通信，可以添加拦截器传递用户上下文。

### 问题4：数据库迁移
**解决方案**：Identity.API使用Entity Framework Core迁移。在Docker启动时自动运行迁移脚本，或使用`DbContext.Database.Migrate()`。

## 扩展功能建议

### 1. 角色和权限管理
- 添加`Admin`、`User`等角色
- 基于角色的访问控制（RBAC）
- 在JWT令牌中包含用户角色

### 2. OAuth2.0/OpenID Connect集成
- 支持第三方登录（Google、GitHub等）
- 实现标准的OAuth2.0授权码流程

### 3. 多因素认证（MFA）
- 支持短信/邮件验证码
- 支持Authenticator应用

### 4. API密钥管理
- 为外部开发者提供API密钥
- 实现速率限制和配额管理

### 5. 审计日志
- 记录用户登录和重要操作
- 实现安全事件监控

## 学习资源

1. **官方文档**
   - [ASP.NET Core Identity](https://learn.microsoft.com/zh-cn/aspnet/core/security/authentication/identity)
   - [JWT Bearer Authentication](https://learn.microsoft.com/zh-cn/aspnet/core/security/authentication/jwtbearer)
   - [YARP Documentation](https://microsoft.github.io/reverse-proxy/)

2. **相关教程**
   - [Microservices Authentication with JWT](https://auth0.com/blog/microservices-authentication-and-authorization-best-practices/)
   - [Secure ASP.NET Core Microservices](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/)

3. **工具和库**
   - [jwt.io](https://jwt.io/) - JWT调试和验证
   - [Postman](https://www.postman.com/) - API测试
   - [RabbitMQ Management UI](http://localhost:15672) - 消息队列监控

## 下一步学习建议

完成身份认证功能后，你可以继续深入学习：

1. **服务间通信**：深入研究gRPC、消息队列的更多高级特性
2. **分布式追踪**：使用OpenTelemetry实现请求链路追踪
3. **容器编排**：学习Kubernetes部署和管理微服务
4. **安全加固**：学习OWASP Top 10，实施更多安全措施
5. **性能优化**：学习缓存策略、数据库优化、负载均衡

---

**祝你学习顺利！如果在实施过程中遇到问题，可以参考现有服务的代码结构，或查阅相关文档。微服务是一个渐进式学习过程，每一步的实践都会加深你的理解。**