using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Validation;

public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    [Fact(DisplayName = "Valid sale item should not have validation errors")]
    public void Should_NotHaveValidationErrors_ForValidSaleItem()
    {
        // Arrange
        var item = new SaleItem("Product A", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Sale item with empty product name should have validation error")]
    public void Should_HaveValidationError_ForEmptyProductName()
    {
        // Arrange
        var item = new SaleItem(string.Empty, 5, 10.00m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(si => si.ProductName)
            .WithErrorMessage("Product name cannot be empty.");
    }

    [Fact(DisplayName = "Sale item with product name exceeding max length should have validation error")]
    public void Should_HaveValidationError_ForProductNameExceedingMaxLength()
    {
        // Arrange
        var longProductName = new string('a', 101);
        var item = new SaleItem(longProductName, 5, 10.00m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(si => si.ProductName)
            .WithErrorMessage("Product name cannot exceed 100 characters.");
    }

    [Theory(DisplayName = "Sale item with invalid quantity should have validation error")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(21)]
    public void Should_HaveValidationError_ForInvalidQuantity(int quantity)
    {
        // Arrange
        var item = new SaleItem("Product A", quantity, 10.00m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(si => si.Quantity);
    }

    [Theory(DisplayName = "Sale item with invalid unit price should have validation error")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_HaveValidationError_ForInvalidUnitPrice(decimal unitPrice)
    {
        // Arrange
        var item = new SaleItem("Product A", 5, unitPrice);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(si => si.UnitPrice)
            .WithErrorMessage("Unit price must be greater than zero.");
    }

    [Fact(DisplayName = "Sale item with negative total amount should have validation error")]
    public void Should_HaveValidationError_ForNegativeTotalAmount()
    {
        // Arrange
        var item = new SaleItem("Product A", 5, 10.00m);
        // Manually set TotalAmount to a negative value for testing purposes
        typeof(SaleItem).GetProperty("TotalAmount")?.SetValue(item, -10.00m);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(si => si.TotalAmount)
            .WithErrorMessage("Total amount cannot be negative.");
    }
}