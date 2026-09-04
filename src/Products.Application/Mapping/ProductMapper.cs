using Products.Application.DTOs.Products;
using Products.Domain.Entities;

namespace Products.Application.Mapping;

/// <summary>
/// Mapeo manual y explícito. Con solo un par de DTOs no se justifica traer
/// AutoMapper como dependencia adicional.
/// </summary>
public static class ProductMapper
{
    public static ProductResponse ToResponse(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };
}