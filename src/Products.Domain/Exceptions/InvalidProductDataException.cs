namespace Products.Domain.Exceptions;

public class InvalidProductDataException : DomainException
{
    public InvalidProductDataException(string message) : base(message) { }
}