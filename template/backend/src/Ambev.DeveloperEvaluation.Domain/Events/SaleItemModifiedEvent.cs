
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleItemModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid SaleItemId { get; }
    public DateTime OccurredOn { get; }

    public SaleItemModifiedEvent(Guid saleId, Guid saleItemId)
    {
        SaleId = saleId;
        SaleItemId = saleItemId;
        OccurredOn = DateTime.UtcNow;
    }
}
