using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Authentication.Application.Service
{
    public static class ApplicationService
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // 1. Get the global configuration object
            var config = TypeAdapterConfig.GlobalSettings;

            // 2. Scan the current assembly for any IRegister implementations
            config.Scan(Assembly.GetExecutingAssembly());

            // 3. Add the configuration to the DI container
            services.AddSingleton(config);

            // 4. THIS IS THE MISSING PIECE: Register the IMapper service
            services.AddScoped<IMapper, ServiceMapper>();


            return services;
        }
    }
}
