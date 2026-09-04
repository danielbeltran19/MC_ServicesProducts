using System.ComponentModel.DataAnnotations;

namespace Products.Application.DTOs.Products;

public class CreateProductRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(1000, ErrorMessage = "La descripción no puede superar 1000 caracteres.")]
    public string Description { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock inicial no puede ser negativo.")]
    public int Stock { get; set; }
}