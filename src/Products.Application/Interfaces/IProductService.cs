using Products.Application.DTOs.Common;
using Products.Application.DTOs.Products;

namespace Products.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResponse<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);

    Task<ProductResponse> UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}