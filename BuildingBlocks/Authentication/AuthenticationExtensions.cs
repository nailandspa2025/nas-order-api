using System.Text;
using BuildingBlocks.Authentication.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BuildingBlocks.Authentication;

public static class AuthenticationExtensions
{
    public static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();

        var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = jwtSettings?.Authority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings?.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var responseMessage = JsonConvert.SerializeObject(new
                        {
                            success = false,
                            message = "You are not Authorized"
                        });
                        return context.Response.WriteAsync(responseMessage);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var responseMessage = JsonConvert.SerializeObject(new
                        {
                            success = false,
                            message = "You are not authorized to access this resource"
                        });
                        return context.Response.WriteAsync(responseMessage);
                    }
                };
            });
    }
}

