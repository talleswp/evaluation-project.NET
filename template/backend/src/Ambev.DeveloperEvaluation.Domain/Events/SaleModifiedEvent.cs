using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; } // Added
    public DateTime OccurredAt { get; } // Changed from OccurredOn

    public SaleModifiedEvent(Guid saleId, string saleNumber, DateTime occurredAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = occurredAt;
    }
}