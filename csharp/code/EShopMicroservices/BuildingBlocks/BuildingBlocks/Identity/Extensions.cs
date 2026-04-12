using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Identity;

/// <summary>
/// 用户上下文依赖注入扩展方法
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 添加用户上下文服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        // 添加 HTTP 上下文访问器（如果尚未添加）
        services.AddHttpContextAccessor();

        // 注册用户上下文为作用域服务
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}