using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Entities;

public class SaleItemTests
{
    [Theory(DisplayName = "Should calculate correct discount for quantity")]
    [InlineData(1, 0.00)] // No discount
    [InlineData(3, 0.00)] // No discount
    [InlineData(4, 0.10)] // 10% discount
    [InlineData(9, 0.10)] // 10% discount
    [InlineData(10, 0.20)] // 20% discount
    [InlineData(20, 0.20)] // 20% discount
    public void Should_CalculateCorrectDiscount_ForQuantity(int quantity, decimal expectedDiscount)
    {
        // Arrange
        var item = new SaleItem("Product", quantity, 10.00m);

        // Assert
        item.Discount.Should().Be(expectedDiscount);
    }

    [Fact(DisplayName = "Should calculate total amount correctly with discount")]
    public void Should_CalculateTotalAmount_CorrectlyWithDiscount()
    {
        // Arrange
        var item = new SaleItem("Product", 10, 10.00m); // 20% discount

        // Assert
        item.TotalAmount.Should().Be(80.00m); // 10 * 10 = 100; 100 - 20% = 80
    }

    [Fact(DisplayName = "Should update quantity and recalculate total amount")]
    public void Should_UpdateQuantity_AndRecalculateTotalAmount()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m); // 10% discount

        // Act
        item.UpdateQuantity(15); // Change to 20% discount

        // Assert
        item.Quantity.Should().Be(15);
        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(120.00m); // 15 * 10 = 150; 150 - 20% = 120
    }

    [Fact(DisplayName = "Should throw DomainException when updating with zero quantity")]
    public void Should_ThrowDomainException_WhenUpdatingWithZeroQuantity()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m);

        // Act
        Action act = () => item.UpdateQuantity(0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Should throw DomainException when updating with negative quantity")]
    public void Should_ThrowDomainException_WhenUpdatingWithNegativeQuantity()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m);

        // Act
        Action act = () => item.UpdateQuantity(-1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Should update unit price and recalculate total amount")]
    public void Should_UpdateUnitPrice_AndRecalculateTotalAmount()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m); // 10% discount

        // Act
        item.UpdateUnitPrice(12.00m);

        // Assert
        item.UnitPrice.Should().Be(12.00m);
        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(54.00m); // 5 * 12 = 60; 60 - 10% = 54
    }

    [Fact(DisplayName = "Should throw DomainException when updating with zero unit price")]
    public void Should_ThrowDomainException_WhenUpdatingWithZeroUnitPrice()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m);

        // Act
        Action act = () => item.UpdateUnitPrice(0);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Unit price must be greater than zero.");
    }

    [Fact(DisplayName = "Should throw DomainException when updating with negative unit price")]
    public void Should_ThrowDomainException_WhenUpdatingWithNegativeUnitPrice()
    {
        // Arrange
        var item = new SaleItem("Product", 5, 10.00m);

        // Act
        Action act = () => item.UpdateUnitPrice(-1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Unit price must be greater than zero.");
    }
}