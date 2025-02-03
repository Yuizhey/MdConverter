using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MdConverter.Application;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<AuthSettings>(configuration.GetSection("AUTH"));

        var authSettings = new AuthSettings
        {
            SecretKey = configuration["AUTH:SECRETKEY"]!,
            Expires = TimeSpan.Parse(configuration["AUTH:EXPIRES"]!)
        };

        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey))
            };
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["token"];
                    return Task.CompletedTask;
                }
            };
        });

        return service;
    }
}