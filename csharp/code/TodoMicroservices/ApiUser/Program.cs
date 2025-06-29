using ApiUser.Application.Extensions;
using ApiUser.Domain.Repositories;
using ApiUser.Extensions;
using ApiUser.Infrastructure.Extensions;
using ApiUser.Infrastructure.Persistence;
using DotNetCore.CAP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. œ»◊¢≤·CAP∫ÕMediatR£®DbContext“¿¿µÀ¸√«£©
//builder.Services.AddCap(options =>
//{
//    options.UseMySql(builder.Configuration.GetConnectionString("mysql")!);
//    options.UseRabbitMQ(options =>
//    {
//        builder.Configuration.GetSection("RabbitMQ").Bind(options);
//    });
//});
//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.AddPresentation();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

//app.Logger.LogInformation("Swagger URL: /swagger/v1/swagger.json");

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;

    try
    {
        var dbOptions = provider.GetService<DbContextOptions<UserDbContext>>();
        var mediator = provider.GetService<IMediator>();
        var capBus = provider.GetService<ICapPublisher>();
        var dbContext = provider.GetService<UserDbContext>();
        var repo = provider.GetService<IUsersRepository>();

        //app.Logger.LogInformation($"DbContextOptions resolved: {dbOptions != null}");
        //app.Logger.LogInformation($"IMediator resolved: {mediator != null}");
        //app.Logger.LogInformation($"ICapPublisher resolved: {capBus != null}");
        //app.Logger.LogInformation($"UserDbContext resolved: {dbContext != null}");
        //app.Logger.LogInformation($"UsersRepository resolved: {repo != null}");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "“¿¿µΩ‚Œˆ ß∞‹");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
