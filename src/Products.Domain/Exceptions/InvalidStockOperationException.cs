namespace Products.Domain.Exceptions;

public class InvalidStockOperationException : DomainException
{
    public InvalidStockOperationException(string message) : base(message) { }
}