
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Validation; 

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handles the command to cancel a sale.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly SaleValidator _saleValidator; 

    public CancelSaleHandler(ISaleRepository saleRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
        _saleValidator = new SaleValidator(); 
    }

    /// <summary>
    /// Handles the <see cref="CancelSaleCommand"/> to cancel a sale.
    /// </summary>
    /// <param name="command">The command to cancel a sale.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the cancel sale operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the sale is not found.</exception>
    /// <exception cref="ValidationException">Thrown when the sale cancellation fails validation.</exception>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found.");
        }

        sale.CancelSale(); 

        var validationResult = _saleValidator.Validate(sale);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors); 
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken); 

        var saleCancelledEvent = new SaleCancelledEvent(sale.Id, sale.SaleNumber);
        _logger.LogInformation("Domain Event: {EventType} - SaleId: {SaleId}, SaleNumber: {SaleNumber}", 
            nameof(SaleCancelledEvent), saleCancelledEvent.SaleId, saleCancelledEvent.SaleNumber);

        return new CancelSaleResult { Id = sale.Id, Success = true };
    }
}
