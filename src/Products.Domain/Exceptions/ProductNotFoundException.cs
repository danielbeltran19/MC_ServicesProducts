namespace Products.Domain.Exceptions;

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(Guid id)
        : base($"No se encontró el producto con id '{id}'.") { }
}