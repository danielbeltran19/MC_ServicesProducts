using Microsoft.Extensions.DependencyInjection;
using Products.Application.Interfaces;
using Products.Application.Services;

namespace Products.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}