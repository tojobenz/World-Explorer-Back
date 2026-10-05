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
    
    // Application Services
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<IEmailService, EmailService>();
    
    // Infrastructure - Database
    services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    
    // Infrastructure - Repositories
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    
    // JWT Authentication
    var jwtSettings = configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
    
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
    
    services.AddAuthorization();
    
    return services;
}
}