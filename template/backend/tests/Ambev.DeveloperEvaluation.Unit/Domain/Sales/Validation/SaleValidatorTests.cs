
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Validation;

public class SaleValidatorTests
{
    private readonly SaleValidator _validator;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
    }

    [Fact(DisplayName = "Valid sale should not have validation errors")]
    public void Should_NotHaveValidationErrors_ForValidSale()
    {
        // Arrange
        var sale = new Sale("Customer Name", "Branch A");
        sale.AddItem("Product 1", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Sale with empty customer name should have validation error")]
    public void Should_HaveValidationError_ForEmptyCustomerName()
    {
        // Arrange
        var sale = new Sale(string.Empty, "Branch A");
        sale.AddItem("Product 1", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Customer)
            .WithErrorMessage("Customer name cannot be empty.");
    }

    [Fact(DisplayName = "Sale with customer name exceeding max length should have validation error")]
    public void Should_HaveValidationError_ForCustomerNameExceedingMaxLength()
    {
        // Arrange
        var longCustomerName = new string('a', 101);
        var sale = new Sale(longCustomerName, "Branch A");
        sale.AddItem("Product 1", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Customer)
            .WithErrorMessage("Customer name cannot exceed 100 characters.");
    }

    [Fact(DisplayName = "Sale with empty branch should have validation error")]
    public void Should_HaveValidationError_ForEmptyBranch()
    {
        // Arrange
        var sale = new Sale("Customer Name", string.Empty);
        sale.AddItem("Product 1", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Branch)
            .WithErrorMessage("Branch cannot be empty.");
    }

    [Fact(DisplayName = "Sale with branch exceeding max length should have validation error")]
    public void Should_HaveValidationError_ForBranchExceedingMaxLength()
    {
        // Arrange
        var longBranch = new string('a', 51);
        var sale = new Sale("Customer Name", longBranch);
        sale.AddItem("Product 1", 5, 10.00m);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Branch)
            .WithErrorMessage("Branch cannot exceed 50 characters.");
    }

    [Fact(DisplayName = "Sale with no items should have validation error")]
    public void Should_HaveValidationError_ForNoItems()
    {
        // Arrange
        var sale = new Sale("Customer Name", "Branch A");

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Items)
            .WithErrorMessage("A sale must have at least one item.");
    }

    [Fact(DisplayName = "Sale with invalid item should have validation error")]
    public void Should_HaveValidationError_ForInvalidItem()
    {
        // Arrange
        var sale = new Sale("Customer Name", "Branch A");
        // Create an invalid SaleItem directly, bypassing AddItem's DomainException
        var invalidItem = new SaleItem(string.Empty, 0, 0);
        // Use reflection to add the invalid item to the private list
        var items = typeof(Sale).GetProperty("Items")?.GetValue(sale) as List<SaleItem>;
        items?.Add(invalidItem);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(s => s.Items)
            .WithErrorMessage("One or more sale items are invalid.");
    }
}
