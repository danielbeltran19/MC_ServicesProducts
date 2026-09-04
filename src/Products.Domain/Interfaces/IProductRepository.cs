using Products.Domain.Common;
using Products.Domain.Entities;

namespace Products.Domain.Interfaces;

// Puerto de salida del dominio hacia la persistencia. La implementación
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    void Update(Product product);

    void Remove(Product product);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);


    // Persiste los cambios pendientes (Unit of Work). Devuelve la cantidad de entidades afectadas.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}