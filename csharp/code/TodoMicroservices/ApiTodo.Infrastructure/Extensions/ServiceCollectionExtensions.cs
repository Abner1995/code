using DotNetCore.CAP;
using ApiTodo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiTodo.Domain.Repositories;
using ApiTodo.Infrastructure.Repositories;

namespace ApiTodo.Infrastructure.Extensions;

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
        services.AddDbContext<TodoDbContext>(options => options.UseMySql(configuration.GetConnectionString("mysql"), new MySqlServerVersion(new Version(5, 6, 20))).EnableSensitiveDataLogging());

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ITodosRepository, TodosRepository>();
    }
}
