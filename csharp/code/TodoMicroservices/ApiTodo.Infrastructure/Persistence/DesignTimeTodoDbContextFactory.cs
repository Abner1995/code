using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApiTodo.Infrastructure.Persistence;

class DesignTimeTodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
{
    public TodoDbContext CreateDbContext(string[] args)
    {
        // 获取当前解决方案的根目录（或手动指定主项目路径）
        var basePath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ApiTodo")
        );
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? "Development";
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true) // 开发/生产环境文件可选
            .Build();

        var builder = new DbContextOptionsBuilder<TodoDbContext>();
        var connectionString = configuration.GetConnectionString("mysql");

        // 根据你的数据库类型配置
        builder.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 6, 20)));

        return new TodoDbContext(builder.Options, null!, null!);
    }
}
