
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Validation; 

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handles the command to create a sale.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly SaleValidator _saleValidator; 

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _saleValidator = new SaleValidator(); 
    }

    /// <summary>
    /// Handles the <see cref="CreateSaleCommand"/> to create a new sale.
    /// </summary>
    /// <param name="command">The command to create a sale.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the create sale operation.</returns>
    /// <exception cref="ValidationException">Thrown when the sale creation fails validation.</exception>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = new Sale(command.Customer, command.Branch);

        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(itemCommand.ProductName, itemCommand.Quantity, itemCommand.UnitPrice);
        }

        var validationResult = _saleValidator.Validate(sale);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors); 
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        var saleCreatedEvent = new SaleCreatedEvent(createdSale.Id, createdSale.SaleNumber);
        _logger.LogInformation("Domain Event: {EventType} - SaleId: {SaleId}, SaleNumber: {SaleNumber}", 
            nameof(SaleCreatedEvent), saleCreatedEvent.SaleId, saleCreatedEvent.SaleNumber);

        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}
