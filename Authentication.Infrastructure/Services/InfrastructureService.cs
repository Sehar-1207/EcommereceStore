using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.DependencyInjection;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        Service.AddSharedServices<AppDbContext>(services, config, config["MySerilog:FileName"]!);

        // Identity for Appuser
        services.AddIdentity<Appuser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Repositories + Token service
        services.AddScoped<IUsers, UserRepository>();
        services.AddScoped<IToken, TokenService>();

        // ✅ JWT auth setup
        services.AddJwtAuthentication(config);


        return services;
    }

    public static IApplicationBuilder AddInfrastructurePolicy(this IApplicationBuilder app)
    {
        app.AddSharedPolicy();
        return app;
    }

    // ✅ Role seeding helper
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
