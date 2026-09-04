using System.ComponentModel.DataAnnotations;

namespace Products.Application.DTOs.Products;

public enum StockOperationType
{
    Increase = 1,
    Decrease = 2
}


//la cantidad siempre es positiva, y el tipo de operación indica el sentido.
public class UpdateStockRequest
{
    [Required(ErrorMessage = "El tipo de operación es obligatorio (Increase o Decrease).")]
    public StockOperationType OperationType { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Quantity { get; set; }
}