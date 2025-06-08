using ApiUser.Domain.Repositories;
using ApiUser.Infrastructure.Persistence;
using ApiUser.Infrastructure.Repositories;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiUser.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserDbContext).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserContextTransactionBehavior<,>));

        // 如果使用 CAP（分布式事务），需要配置
        services.AddCap(options =>
        {
            //options.UseEntityFramework<UserDbContext>();
            options.UseMySql(configuration.GetConnectionString("mysql")!);
            options.UseRabbitMQ(options =>
            {
                configuration.GetSection("RabbitMQ").Bind(options);
            });
        });

        //UserDbContext构造函数依赖注入DbContextOptions options, IMediator mediator, ICapPublisher capBus，注入UserDbContext之前需要按顺序把IMediator,ICapPublisher注入
        services.AddDbContext<UserDbContext>(options => options.UseMySql(configuration.GetConnectionString("mysql"), new MySqlServerVersion(new Version(5, 6, 20))).EnableSensitiveDataLogging());
        // 注册 UserDbContext 的工厂方法
        services.AddScoped<UserDbContext>(provider =>
        {
            var options = provider.GetRequiredService<DbContextOptions<UserDbContext>>();
            var mediator = provider.GetRequiredService<IMediator>();
            var capBus = provider.GetRequiredService<ICapPublisher>();

            if (mediator == null) Console.WriteLine("IMediator未注册！");
            if (capBus == null) Console.WriteLine("ICapPublisher未注册！");

            return new UserDbContext(options, mediator, capBus);
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
