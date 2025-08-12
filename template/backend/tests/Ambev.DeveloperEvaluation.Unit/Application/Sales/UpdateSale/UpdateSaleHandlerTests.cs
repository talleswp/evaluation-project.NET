
using NSubstitute.Core.Arguments;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Should update an existing sale successfully")]
    public async Task Should_UpdateExistingSale_Successfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var existingSale = new Sale("Old Customer", "Old Branch");
        existingSale.AddItem("Product X", 5, 10.00m);
        
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            Customer = "New Customer",
            Branch = "New Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand { Id = existingSale.Items.First().Id, ProductName = "Product X", Quantity = 10, UnitPrice = 12.00m },
                new UpdateSaleItemCommand { ProductName = "Product Y", Quantity = 2, UnitPrice = 5.00m }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(new UpdateSaleResult { Id = saleId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        existingSale.Customer.Should().Be(command.Customer);
        existingSale.Branch.Should().Be(command.Branch);
        existingSale.Items.Should().HaveCount(2);
        existingSale.Items.First(i => i.ProductName == "Product X").Quantity.Should().Be(10);
        existingSale.Items.First(i => i.ProductName == "Product Y").Quantity.Should().Be(2);
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
                                _logger.Received(1).Log(LogLevel.Information, Arg.Any<EventId>(), Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(l => true), Arg.Any<Exception>(), Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale not found")]
    public async Task Should_ThrowKeyNotFoundException_WhenSaleNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand { Id = saleId, Customer = "New Customer", Branch = "New Branch" };
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found.");
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when updating a cancelled sale")]
    public async Task Should_ThrowInvalidOperationException_WhenUpdatingCancelledSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var existingSale = new Sale("Old Customer", "Old Branch");
        existingSale.CancelSale(); // Mark as cancelled

        var command = new UpdateSaleCommand { Id = saleId, Customer = "New Customer", Branch = "New Branch" };
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale.");
    }

    [Fact(DisplayName = "Should throw ValidationException for invalid command")]
    public async Task Should_ThrowValidationException_ForInvalidCommand()
    {
        // Arrange
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            Customer = "", // Invalid customer
            Branch = "", // Invalid branch
            Items = new List<UpdateSaleItemCommand>() // No items
        };

        // Mock GetByIdAsync to return a valid sale so validation can proceed
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(new Sale("Valid Customer", "Valid Branch"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
