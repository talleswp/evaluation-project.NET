using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCreatedEvent : IDomainEvent
{
    public Guid SaleId { get; } 
    public string SaleNumber { get; } 
    public DateTime OccurredAt { get; } // Changed from OccurredOn

    public SaleCreatedEvent(Guid saleId, string saleNumber, DateTime occurredAt)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        OccurredAt = occurredAt;
    }
}