using Ambev.DeveloperEvaluation.Domain.Entities;
using System;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleItemModifiedEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Guid ItemId { get; }
    public string ProductName { get; } // Added
    public int Quantity { get; } // Added
    public DateTime OccurredAt { get; } // Changed from OccurredOn
    public string ModificationType { get; } // Added

    public SaleItemModifiedEvent(Guid saleId, Guid itemId, string productName, int quantity, DateTime occurredAt, string modificationType)
    {
        SaleId = saleId;
        ItemId = itemId;
        ProductName = productName;
        Quantity = quantity;
        OccurredAt = occurredAt;
        ModificationType = modificationType;
    }
}