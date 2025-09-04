using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLibrary.Middleware;

namespace SharedLibrary.DependencyInjection
{
    public static class Service
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string filename) where TContext : DbContext
        {
            // Register generic database shared services here
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Default"),
                sqlServerOption => sqlServerOption.EnableRetryOnFailure()));

            //serilog service registration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{filename}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{TimeStamp:yyyy:MM:dd HH:mm:ss.fff.zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // services.AddTransient<IMyService, MyService>();
            JwtAuthentication.AddJwtAuthentication(services, config);

            return services;
        }

        public static IApplicationBuilder AddSharedPolicy(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptions>();
            //app.UseMiddleware<ListenToApiGateway>();

            return app;
        }

    }
}