
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public DateTime OccurredOn { get; }

    public SaleModifiedEvent(Guid saleId)
    {
        SaleId = saleId;
        OccurredOn = DateTime.UtcNow;
    }
}
