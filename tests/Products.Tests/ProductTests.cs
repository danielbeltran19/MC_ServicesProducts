using FluentAssertions;
using Products.Domain.Entities;
using Products.Domain.Exceptions;
using Xunit;

namespace Products.Tests;

public class ProductTests
{
    [Fact]
    public void Create_ConDatosValidos_CreaElProducto()
    {
        var product = Product.Create("Camiseta", "Camiseta de algodón", 25000m, 10);

        product.Name.Should().Be("Camiseta");
        product.Stock.Should().Be(10);
        product.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_ConStockInicialNegativo_LanzaExcepcion(int stockInvalido)
    {
        var act = () => Product.Create("Camiseta", "Descripción", 25000m, stockInvalido);

        act.Should().Throw<InvalidProductDataException>();
    }

    [Fact]
    public void Create_ConPrecioNegativo_LanzaExcepcion()
    {
        var act = () => Product.Create("Camiseta", "Descripción", -1m, 10);

        act.Should().Throw<InvalidProductDataException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ConNombreVacio_LanzaExcepcion(string? nombreInvalido)
    {
        var act = () => Product.Create(nombreInvalido!, "Descripción", 100m, 10);

        act.Should().Throw<InvalidProductDataException>();
    }

    [Fact]
    public void AdjustStock_SumandoUnidades_IncrementaElStock()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 10);

        product.AdjustStock(5);

        product.Stock.Should().Be(15);
    }

    [Fact]
    public void AdjustStock_RestandoUnidadesDentroDelDisponible_DecrementaElStock()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 10);

        product.AdjustStock(-4);

        product.Stock.Should().Be(6);
    }

    [Fact]
    public void AdjustStock_RestandoMasDeLoDisponible_LanzaInsufficientStockException()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 5);

        var act = () => product.AdjustStock(-10);

        act.Should().Throw<InsufficientStockException>();
        product.Stock.Should().Be(5, "el stock no debe cambiar si la operación falla");
    }

    [Fact]
    public void AdjustStock_DejandoElStockExactamenteEnCero_NoLanzaExcepcion()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 5);

        product.AdjustStock(-5);

        product.Stock.Should().Be(0);
    }

    [Fact]
    public void AdjustStock_ConDeltaCero_LanzaInvalidStockOperationException()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 5);

        var act = () => product.AdjustStock(0);

        act.Should().Throw<InvalidStockOperationException>();
    }

    [Fact]
    public void UpdateDetails_ActualizaNombreDescripcionYPrecio()
    {
        var product = Product.Create("Camiseta", "Descripción", 25000m, 5);

        product.UpdateDetails("Camiseta Premium", "Nueva descripción", 30000m);

        product.Name.Should().Be("Camiseta Premium");
        product.Description.Should().Be("Nueva descripción");
        product.Price.Should().Be(30000m);
        product.UpdatedAt.Should().NotBeNull();
    }
}