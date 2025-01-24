using CleanArchitecture.Application.Restaurants;
using CleanArchitecture.Application.Users;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddScoped<IRestaurantsService, RestaurantsService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddAutoMapper(applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
           .AddFluentValidationAutoValidation();

        services.AddScoped<IUserContext, UserContext>();
        //UserContext使用到IHttpContextAccessor所以需要注入
        services.AddHttpContextAccessor();
    }
}
