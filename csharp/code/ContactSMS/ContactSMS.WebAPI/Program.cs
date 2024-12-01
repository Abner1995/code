using AspNetCoreRateLimit;
using ContactSMS.WebAPI.Constants;
using ContactSMS.WebAPI.HealthCheck;
using HealthChecks.UI.Client;
using Learn.WebAPI.StartupConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlX.XDevAPI;
using System.Text;
using Ubiety.Dns.Core;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<RandomHealthCheck>("网站检测");
builder.Services.AddHealthChecksUI(opts =>
{
    opts.AddHealthCheckEndpoint("api", "health");
    opts.SetEvaluationTimeInSeconds(5);
    opts.SetMinimumSecondsBetweenFailureNotifications(10);
}).AddInMemoryStorage();
//builder.Services.AddWatchDogServices();
builder.Services.AddResponseCaching();

builder.Services.AddMemoryCache();
builder.AddRateLimitServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    var title = "API版本";
    var desc = "API版本";
    var terms = new Uri("http://localhost:5220");
    var license = new OpenApiLicense()
    {
        Name = "许可证"
    };
    var contact = new OpenApiContact()
    {
        Name = "xuzizheng",
        Email = "wan19950504@gmail.com",
        Url = terms,
    };
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = $"{title} v1(deprecated)",
        Description = desc,
        TermsOfService = terms,
        License = license,
        Contact = contact
    });
    opts.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = $"{title} v2",
        Description = desc,
        TermsOfService = terms,
        License = license,
        Contact = contact
    });
    var securityScheme = new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "JWT Authorization Header info using bearer tokens",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearerAuth"
                }
            },
            new string[] {}
        }
    };
    opts.AddSecurityDefinition("bearerAuth", securityScheme);
    opts.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddApiVersioning(opts =>
{
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.DefaultApiVersion = new(1, 0);
    opts.ReportApiVersions = true;
}).AddApiExplorer(opts =>
{
    opts.GroupNameFormat = "'v'VVV";
    opts.SubstituteApiVersionInUrl = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HealthCheck", policy =>
    {
        policy.RequireAssertion(_ => true); // 允许所有访问
    });
    options.AddPolicy(PolicyContstants.MustHaveEmployeeId, policy =>
    {
        policy.RequireClaim("employeeId");
    });
    options.AddPolicy(PolicyContstants.MustBeTheOwner, policy =>
    {
        policy.RequireClaim("title", "Business owner");
    });
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("Authentication:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Authentication:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:SecretKey").Value!))
        };
    });

var app = builder.Build();

//app.UseWatchDogExceptionLogger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        opts.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.UseIpRateLimiting();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).RequireAuthorization("HealthCheck");
app.MapHealthChecksUI().RequireAuthorization("HealthCheck");

//app.UseWatchDog(opt =>
//{
//    opt.WatchPageUsername = app.Configuration.GetValue<string>("WatchDog:UserName");
//    opt.WatchPagePassword = app.Configuration.GetValue<string>("WatchDog:PassWord");
//});

app.Run();