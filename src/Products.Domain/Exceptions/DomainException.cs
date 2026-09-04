namespace Products.Domain.Exceptions;

/// Excepción base para todas las violaciones de reglas de negocio del dominio.
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}