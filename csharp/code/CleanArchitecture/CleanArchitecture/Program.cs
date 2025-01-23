using CleanArchitecture.Infrastructure.Extensions;
using CleanArchitecture.Infrastructure.Seeders;
using CleanArchitecture.Application.Extensions;
using Serilog;
using Serilog.Events;
using CleanArchitecture.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeLoggingMiddleware>();
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
//configuration
//.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
//.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
//.WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}| {NewLine}{Message:lj}{NewLine}{Exception}")
);

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();
await seeder.Seed();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeLoggingMiddleware>();

app.UseSerilogRequestLogging();

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
