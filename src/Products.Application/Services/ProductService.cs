using Microsoft.Extensions.Logging;
using Products.Application.DTOs.Common;
using Products.Application.DTOs.Products;
using Products.Application.Interfaces;
using Products.Application.Mapping;
using Products.Domain.Entities;
using Products.Domain.Exceptions;
using Products.Domain.Interfaces;

namespace Products.Application.Services;

public class ProductService : IProductService
{
    private const int MaxPageSize = 100;

    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(request.Name, request.Description, request.Price, request.Stock);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Producto {ProductId} creado con stock inicial {Stock}", product.Id, product.Stock);

        return product.ToResponse();
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        return product.ToResponse();
    }

    public async Task<PagedResponse<ProductResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, MaxPageSize);

        var paged = await _repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

        return new PagedResponse<ProductResponse>
        {
            Items = paged.Items.Select(p => p.ToResponse()).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount,
            TotalPages = paged.TotalPages
        };
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        product.UpdateDetails(request.Name, request.Description, request.Price);

        _repository.Update(product);
        await _repository.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }

    public async Task<ProductResponse> UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        var delta = request.OperationType == StockOperationType.Increase
            ? request.Quantity
            : -request.Quantity;

        product.AdjustStock(delta);

        _repository.Update(product);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Stock del producto {ProductId} ajustado en {Delta}. Nuevo stock: {Stock}",
            product.Id, delta, product.Stock);

        return product.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ProductNotFoundException(id);

        _repository.Remove(product);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}