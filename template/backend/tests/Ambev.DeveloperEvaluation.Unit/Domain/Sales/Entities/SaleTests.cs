
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Should create a new sale with generated sale number and current date")]
    public void Should_CreateNewSale_WithGeneratedSaleNumberAndCurrentDate()
    {
        // Arrange
        var customer = "Test Customer";
        var branch = "Test Branch";

        // Act
        var sale = new Sale(customer, branch);

        // Assert
        sale.Should().NotBeNull();
        sale.Id.Should().NotBeEmpty();
        sale.SaleNumber.Should().StartWith("SALE-");
        sale.SaleDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        sale.Customer.Should().Be(customer);
        sale.Branch.Should().Be(branch);
        sale.IsCancelled.Should().BeFalse();
        sale.TotalAmount.Should().Be(0);
        sale.Items.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should add a new item to the sale and calculate total amount")]
    public void Should_AddItemToSale_AndCalculateTotalAmount()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        var productName = "Product A";
        var quantity = 5;
        var unitPrice = 10.00m;

        // Act
        sale.AddItem(productName, quantity, unitPrice);

        // Assert
        sale.Items.Should().HaveCount(1);
        var item = sale.Items.First();
        item.ProductName.Should().Be(productName);
        item.Quantity.Should().Be(quantity);
        item.UnitPrice.Should().Be(unitPrice);
        item.Discount.Should().Be(0.10m); // 10% discount for 5 items
        item.TotalAmount.Should().Be(45.00m); // 5 * 10 - 10%
        sale.TotalAmount.Should().Be(45.00m);
    }

    [Fact(DisplayName = "Should update quantity of existing item and recalculate total amount")]
    public void Should_UpdateQuantityOfExistingItem_AndRecalculateTotalAmount()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 3, 10.00m);
        var existingItem = sale.Items.First();

        // Act
        sale.AddItem("Product A", 2, 10.00m); // Add 2 more, total 5

        // Assert
        sale.Items.Should().HaveCount(1);
        existingItem.Quantity.Should().Be(5);
        existingItem.Discount.Should().Be(0.10m); // 10% discount for 5 items
        existingItem.TotalAmount.Should().Be(45.00m);
        sale.TotalAmount.Should().Be(45.00m);
    }

    [Fact(DisplayName = "Should throw exception when adding more than 20 identical items")]
    public void Should_ThrowException_WhenAddingMoreThan20IdenticalItems()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 15, 10.00m);

        // Act
        Action act = () => sale.AddItem("Product A", 6, 10.00m); // Total 21

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Cannot sell more than 20 identical items for product 'Product A'. Current: 15, Adding: 6.");
    }

    [Fact(DisplayName = "Should cancel a sale")]
    public void Should_CancelSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");

        // Act
        sale.CancelSale();

        // Assert
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Should throw exception when cancelling an already cancelled sale")]
    public void Should_ThrowException_WhenCancellingAlreadyCancelledSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.CancelSale();

        // Act
        Action act = () => sale.CancelSale();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Sale is already cancelled.");
    }

    [Fact(DisplayName = "Should update an existing item in the sale")]
    public void Should_UpdateExistingItemInSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 5, 10.00m);
        var itemId = sale.Items.First().Id;

        // Act
        sale.UpdateItem(itemId, 10, 12.00m);

        // Assert
        var updatedItem = sale.Items.First();
        updatedItem.Quantity.Should().Be(10);
        updatedItem.UnitPrice.Should().Be(12.00m);
        updatedItem.Discount.Should().Be(0.20m); // 20% discount for 10 items
        updatedItem.TotalAmount.Should().Be(96.00m); // 10 * 12 - 20%
        sale.TotalAmount.Should().Be(96.00m);
    }

    [Fact(DisplayName = "Should remove an item from the sale")]
    public void Should_RemoveItemFromSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 5, 10.00m);
        var itemId = sale.Items.First().Id;

        // Act
        sale.RemoveItem(itemId);

        // Assert
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Should throw exception when removing item from cancelled sale")]
    public void Should_ThrowException_WhenRemovingItemFromCancelledSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 5, 10.00m);
        sale.CancelSale();
        var itemId = sale.Items.First().Id;

        // Act
        Action act = () => sale.RemoveItem(itemId);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Cannot remove items from a cancelled sale.");
    }

    [Fact(DisplayName = "Should throw exception when updating item of cancelled sale")]
    public void Should_ThrowException_WhenUpdatingItemOfCancelledSale()
    {
        // Arrange
        var sale = new Sale("Customer", "Branch");
        sale.AddItem("Product A", 5, 10.00m);
        sale.CancelSale();
        var itemId = sale.Items.First().Id;

        // Act
        Action act = () => sale.UpdateItem(itemId, 6, 11.00m);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Cannot update items of a cancelled sale.");
    }
}
