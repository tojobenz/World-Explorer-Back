using AuthApi.Application.Interfaces;
using AuthApi.Application.Services;
using AuthApi.Domain.Interfaces;
using AuthApi.Infrastructure.Data;
using AuthApi.Infrastructure.Repositories;
using AuthApi.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;

namespace AuthApi.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(AuthenticationService).Assembly);
        
        // Domain Services
        services.AddScoped<IAuthService, AuthService>();
        
        // Application Services - Auth
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // Application Services - Wikimedia
        services.AddScoped<IWikimediaPlaceService, WikimediaPlaceService>();
        services.AddScoped<IWikimediaPersonService, WikimediaPersonService>();
        services.AddScoped<IWikimediaMonumentService, WikimediaMonumentService>();
        services.AddScoped<IWikimediaFactService, WikimediaFactService>();
        
        // Infrastructure - Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        
        // Infrastructure - Repositories - Auth
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        // Infrastructure - Repositories - Wikimedia
        services.AddScoped<IWikimediaPlaceRepository, WikimediaPlaceRepository>();
        services.AddScoped<IWikimediaPersonRepository, WikimediaPersonRepository>();
        services.AddScoped<IWikimediaMonumentRepository, WikimediaMonumentRepository>();
        services.AddScoped<IWikimediaFactRepository, WikimediaFactRepository>();
        
        // Infrastructure - External Services
        services.AddScoped<IWikimediaService, WikimediaService>();
        services.AddHttpClient<IWikimediaService, WikimediaService>();
        
        // JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        {
            KeyId = "AuthApiKeyId"
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"] ?? "AuthApi",
                ValidAudience = jwtSettings["Audience"] ?? "AuthApiUsers",
                IssuerSigningKey = symmetricKey,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        services.AddAuthorization();
        
        return services;
    }
}