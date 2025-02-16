using Contact.Domain.Repositories;
using Contact.Infrastructure.Persistence;
using Contact.Infrastructure.Repositories;
using Contact.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contact.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ContactDbContexts>(options=> options.UseMySql(configuration.GetConnectionString("mysql"), new MySqlServerVersion(new Version(5, 6, 20))).EnableSensitiveDataLogging());

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IContactsRepository, ContactsRepository>();
        services.AddScoped<IPhonesRepository, PhonesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        services.AddScoped<IUserSeeder, UserSeeder>();
    }
}
