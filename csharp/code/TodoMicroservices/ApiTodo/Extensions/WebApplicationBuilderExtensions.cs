using ApiTodo.Infrastructure.Constants;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Todo.Core.Middleware;

namespace ApiTodo.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opts =>
        {
            var title = "Todo MicrService User API";
            var desc = "Todo MicrService User API";
            var terms = new Uri("http://localhost:5274");
            var license = new OpenApiLicense()
            {
                Name = "MIT"
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
                Title = $"{title} v1",
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

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Authentication"));
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:Secret").Value!))
                };
            });

        builder.Services.AddScoped<ErrorHandlingMiddleware>();
    }
}
