using Contact.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contact.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ContactDbContexts>(options=> options.UseMySql(configuration.GetConnectionString("mysql"), new MySqlServerVersion(new Version(5, 6, 20))));
    }
}
