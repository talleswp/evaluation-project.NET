using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public DateTime OccurredAt { get; } // Changed from OccurredOn

    public SaleCancelledEvent(Guid saleId, string saleNumber, DateTime occurredAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = occurredAt;
    }
}