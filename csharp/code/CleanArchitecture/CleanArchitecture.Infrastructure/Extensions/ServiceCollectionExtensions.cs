using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RestaurantsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("RestaurantsDb"))
          .EnableSensitiveDataLogging());

        services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
    }
}
