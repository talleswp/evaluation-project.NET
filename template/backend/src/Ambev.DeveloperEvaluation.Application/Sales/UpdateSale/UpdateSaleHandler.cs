
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Validation; 

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handles the command to update a sale.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly SaleValidator _saleValidator; 

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _saleValidator = new SaleValidator(); 
    }

    /// <summary>
    /// Handles the <see cref="UpdateSaleCommand"/> to update an existing sale.
    /// </summary>
    /// <param name="command">The command to update a sale.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the update sale operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the sale is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when trying to update a cancelled sale.</exception>
    /// <exception cref="ValidationException">Thrown when the sale update fails validation.</exception>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found.");
        }

        if (sale.IsCancelled)
        {
            throw new InvalidOperationException("Cannot update a cancelled sale.");
        }

        sale.UpdateDetails(command.Customer, command.Branch);

        // Handle items: add new, update existing, remove missing
        var existingItemIds = sale.Items.Select(item => item.Id).ToList();
        var commandItemIds = command.Items.Where(item => item.Id.HasValue).Select(item => item.Id!.Value).ToList();

        // Remove items not in command
        foreach (var existingItemId in existingItemIds.Except(commandItemIds))
        {
            sale.RemoveItem(existingItemId);
        }

        // Add or update items
        foreach (var itemCommand in command.Items)
        {
            if (itemCommand.Id.HasValue && existingItemIds.Contains(itemCommand.Id.Value))
            {
                // Update existing item
                sale.UpdateItem(itemCommand.Id.Value, itemCommand.Quantity, itemCommand.UnitPrice);
            }
            else
            {
                // Add new item
                sale.AddItem(itemCommand.ProductName, itemCommand.Quantity, itemCommand.UnitPrice);
            }
        }

        var validationResult = _saleValidator.Validate(sale);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors); 
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleModifiedEvent = new SaleModifiedEvent(updatedSale.Id);
        _logger.LogInformation("Domain Event: {EventType} - SaleId: {SaleId}", 
            nameof(SaleModifiedEvent), saleModifiedEvent.SaleId);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
