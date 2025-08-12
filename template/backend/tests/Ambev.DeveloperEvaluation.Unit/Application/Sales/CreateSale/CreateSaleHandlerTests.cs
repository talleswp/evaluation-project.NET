using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using MediatR; // Added
using Ambev.DeveloperEvaluation.Domain.Events; // Added

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IPublisher _publisher;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _publisher = Substitute.For<IPublisher>(); // Added
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger, _publisher);
    }

    [Fact(DisplayName = "Should create a new sale successfully")]
    public async Task Should_CreateNewSale_Successfully()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            Customer = "Test Customer",
            Branch = "Test Branch",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand { ProductName = "Product A", Quantity = 5, UnitPrice = 10.00m }
            }
        };

        var sale = new Sale(command.Customer, command.Branch);
        sale.AddItem("Product A", 5, 10.00m);

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        response.SaleNumber.Should().Be(sale.SaleNumber);
        response.TotalAmount.Should().Be(sale.TotalAmount);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw ValidationException for invalid command")]
    public async Task Should_ThrowValidationException_ForInvalidCommand()
    {
        // Given
        var command = new CreateSaleCommand
        {
            Customer = "", // Invalid customer
            Branch = "", // Invalid branch
            Items = new List<CreateSaleItemCommand>() // No items
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
