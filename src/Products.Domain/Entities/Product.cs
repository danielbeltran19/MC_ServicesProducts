using Products.Domain.Exceptions;

namespace Products.Domain.Entities;

/// Entidad raíz del agregado Producto. Encapsula sus propias invariantes:
/// el stock nunca puede quedar en negativo y el precio nunca puede ser negativo.
public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core necesita un constructor sin parámetros (privado para no exponerlo).
    private Product() { }

    private Product(string name, string description, decimal price, int initialStock)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetInitialStock(initialStock);
        CreatedAt = DateTime.UtcNow;
    }

 
    /// Factory method: punto único de creación, garantiza que nunca exista
    /// un Product en un estado inválido.
    public static Product Create(string name, string description, decimal price, int initialStock)
        => new(name, description, price, initialStock);

    public void UpdateDetails(string name, string description, decimal price)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        Touch();
    }

    /// Ajusta el stock actual sumando o restando unidades (delta positivo o negativo).
    /// Lanza InsufficientStockException si la operación dejaría el stock en negativo.

    public void AdjustStock(int delta)
    {
        if (delta == 0)
            throw new InvalidStockOperationException("La cantidad a ajustar no puede ser cero.");

        var newStock = Stock + delta;

        if (newStock < 0)
            throw new InsufficientStockException(Id, Stock, delta);

        Stock = newStock;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidProductDataException("El nombre del producto es obligatorio.");

        if (name.Length > 150)
            throw new InvalidProductDataException("El nombre del producto no puede superar 150 caracteres.");

        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidProductDataException("La descripción del producto es obligatoria.");

        Description = description.Trim();
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new InvalidProductDataException("El precio no puede ser negativo.");

        Price = price;
    }

    private void SetInitialStock(int initialStock)
    {
        if (initialStock < 0)
            throw new InvalidProductDataException("El stock inicial no puede ser negativo.");

        Stock = initialStock;
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}