using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Infrastructure.Data;
using Products.Catalogue.Infrastructure.Repositories;
using SharedLibrary.DependencyInjection;

namespace ProductAPi.Infrastructure.DependecyInjection
{
    public static class InfrastructureService
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            // Add shared services
            Service.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            //services.AddScoped<ProductRepository>();
            services.AddScoped<IProduct, ProductRepository>();
            services.AddScoped<ICategory, CategoryRepository>();
            services.AddScoped<IBrand, BrandRepository>();
            return services;
        }

        public static IApplicationBuilder AddInfrastructurePolicy(this IApplicationBuilder app)
        {
            app.AddSharedPolicy();
            return app;
        }
    }
}
