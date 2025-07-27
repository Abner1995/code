using ApiAdmin.Domain.Repositories;
using ApiAdmin.Infrastructure.Persistence;
using ApiAdmin.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAdmin.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 如果使用 CAP（分布式事务），需要配置
        services.AddCap(options =>
        {
            options.UseMySql(configuration.GetConnectionString("mysql")!);
            options.UseRabbitMQ(options =>
            {
                configuration.GetSection("RabbitMQ").Bind(options);
            });
        });
        services.AddDbContext<AdminDbContext>(options => options.UseMySql(configuration.GetConnectionString("mysql"), new MySqlServerVersion(new Version(5, 6, 20))).EnableSensitiveDataLogging());

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IAdminsRepository, AdminsRepository>();
    }
}
