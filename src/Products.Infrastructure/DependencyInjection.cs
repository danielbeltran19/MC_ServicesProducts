using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Domain.Interfaces;
using Products.Infrastructure.Persistence;
using Products.Infrastructure.Repositories;

namespace Products.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // La cadena de conexión puede venir de appsettings.json (dev local)
        // o sobrescribirse por variable de entorno ConnectionStrings__ProductsDb
        // (usado en Docker / despliegue), según convención estándar de .NET.
        var connectionString = configuration.GetConnectionString("ProductsDb")
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'ProductsDb'.");

        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.EnableRetryOnFailure(maxRetryCount: 3)));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}