namespace Products.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public Guid ProductId { get; }
    public int CurrentStock { get; }
    public int RequestedDelta { get; }

    public InsufficientStockException(Guid productId, int currentStock, int requestedDelta)
        : base($"Stock insuficiente para el producto {productId}. " +
               $"Stock actual: {currentStock}, ajuste solicitado: {requestedDelta}.")
    {
        ProductId = productId;
        CurrentStock = currentStock;
        RequestedDelta = requestedDelta;
    }
}