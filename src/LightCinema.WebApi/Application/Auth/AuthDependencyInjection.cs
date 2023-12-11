using System.Security.Claims;
using System.Text;
using LightCinema.WebApi.Application.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LightCinema.WebApi.Application.Auth;

public static class AuthDependencyInjection
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.Bind("Jwt", jwtConfig);
        services.AddSingleton(jwtConfig);
        
        services.AddAuthentication(config =>
            {
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.SaveToken = true;
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                    };
                }
            );
        
        return services;
    }
    
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.RequireAdministratorRole, policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, RoleNames.Visitor, RoleNames.Admin);
            });
                
            options.AddPolicy(PolicyNames.RequireVisitorRole, policy =>
            {
                policy.RequireClaim(ClaimTypes.Role, RoleNames.Visitor);
            });
        });
        
        return services;
    }
}