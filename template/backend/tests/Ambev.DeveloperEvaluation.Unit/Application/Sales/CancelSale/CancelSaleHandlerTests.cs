using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using MediatR; // Added
using Ambev.DeveloperEvaluation.Domain.Events; // Added

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly IPublisher _publisher;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _publisher = Substitute.For<IPublisher>();
        _handler = new CancelSaleHandler(_saleRepository, _logger, _publisher);
    }

    [Fact(DisplayName = "Should cancel sale successfully")]
    public async Task Should_CancelSale_Successfully()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale("Customer", "Branch");
        sale.Id = saleId; // Ensure the mocked sale has the correct ID
        sale.AddItem("Product A", 5, 10.00m);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelSaleCommand(saleId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        result.Success.Should().BeTrue();
        sale.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale not found")]
    public async Task Should_ThrowKeyNotFoundException_WhenSaleNotFound()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null);
        var command = new CancelSaleCommand(saleId);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found.");
    }

    [Fact(DisplayName = "Should throw ValidationException for invalid command")]
    public async Task Should_ThrowValidationException_ForInvalidCommand()
    {
        // Arrange
        var command = new CancelSaleCommand(Guid.Empty); // Invalid command

        // Mock GetByIdAsync to return a valid sale so validation can proceed
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(new Sale("Valid Customer", "Valid Branch"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}