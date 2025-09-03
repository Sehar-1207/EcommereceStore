using Mapster;
using MapsterMapper; // You need this using statement
using Microsoft.Extensions.DependencyInjection;
using Products.Catalogue.Application.Interfaces; // For IProductService
using System.Reflection;

namespace Products.Catalogue.Application.Services
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 1. Get the global configuration object
            var config = TypeAdapterConfig.GlobalSettings;

            // 2. Scan the current assembly for any IRegister implementations
            config.Scan(Assembly.GetExecutingAssembly());

            // 3. Add the configuration to the DI container
            services.AddSingleton(config);

            // 4. THIS IS THE MISSING PIECE: Register the IMapper service
            services.AddScoped<IMapper, ServiceMapper>();

            // 5. Register your custom services
            // Best practice is to register the interface, not just the class
            //services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}