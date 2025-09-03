using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SharedLibrary.DependencyInjection
{
    public static class JwtAuthentication
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // ✅ Use "Authentication" section (matches your JSON)
            var jwtSection = config.GetSection("Authentication");

            var key = jwtSection["key"]
                ?? throw new InvalidOperationException("JWT Key not configured in appsettings.json.");
            var issuer = jwtSection["issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not configured in appsettings.json.");
            var audience = jwtSection["audience"]
                ?? throw new InvalidOperationException("JWT Audience not configured in appsettings.json.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // 🔒 set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = signingKey
                };
            });

            return services;
        }
    }
}
