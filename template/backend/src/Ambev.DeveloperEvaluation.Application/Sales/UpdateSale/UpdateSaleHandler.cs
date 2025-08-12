using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Validation; 
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handles the command to update a sale.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IPublisher _publisher; // Add IPublisher
    private readonly SaleValidator _saleValidator; 

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger, IPublisher publisher) // Inject IPublisher
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _publisher = publisher; // Assign IPublisher
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
            var removedItem = sale.Items.FirstOrDefault(i => i.Id == existingItemId);
            sale.RemoveItem(existingItemId);
            if (removedItem != null)
            {
                var itemModifiedEvent = new SaleItemModifiedEvent(sale.Id, removedItem.Id, removedItem.ProductName, removedItem.Quantity, DateTime.UtcNow, "Removed");
                await _publisher.Publish(itemModifiedEvent, cancellationToken);
            }
        }

        // Add or update items
        foreach (var itemCommand in command.Items)
        {
            if (itemCommand.Id.HasValue && existingItemIds.Contains(itemCommand.Id.Value))
            {
                // Update existing item
                var originalQuantity = sale.Items.FirstOrDefault(i => i.Id == itemCommand.Id.Value)?.Quantity;
                sale.UpdateItem(itemCommand.Id.Value, itemCommand.Quantity, itemCommand.UnitPrice);
                var updatedItem = sale.Items.FirstOrDefault(i => i.Id == itemCommand.Id.Value);
                if (updatedItem != null)
                {
                    var itemModifiedEvent = new SaleItemModifiedEvent(sale.Id, updatedItem.Id, updatedItem.ProductName, updatedItem.Quantity, DateTime.UtcNow, "Updated");
                    await _publisher.Publish(itemModifiedEvent, cancellationToken);
                }
            }
            else
            {
                // Add new item
                sale.AddItem(itemCommand.ProductName, itemCommand.Quantity, itemCommand.UnitPrice);
                var addedItem = sale.Items.LastOrDefault(); // Assuming it's the last added
                if (addedItem != null)
                {
                    var itemModifiedEvent = new SaleItemModifiedEvent(sale.Id, addedItem.Id, addedItem.ProductName, addedItem.Quantity, DateTime.UtcNow, "Added");
                    await _publisher.Publish(itemModifiedEvent, cancellationToken);
                }
            }
        }

        var validationResult = _saleValidator.Validate(sale);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors); 
        }

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleModifiedEvent = new SaleModifiedEvent(updatedSale.Id, updatedSale.SaleNumber, DateTime.UtcNow); // Add SaleNumber and OccurredAt
        await _publisher.Publish(saleModifiedEvent, cancellationToken); // Publish the event

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}