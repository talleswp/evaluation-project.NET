using MediatR;
using Serilog;
using Ambev.DeveloperEvaluation.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    public class SaleItemModifiedEventHandler : INotificationHandler<SaleItemModifiedEvent>
    {
        public Task Handle(SaleItemModifiedEvent notification, CancellationToken cancellationToken)
        {
            Log.Information($"Domain Event: SaleItemModifiedEvent - SaleId: {notification.SaleId}, ItemId: {notification.ItemId}, ProductName: {notification.ProductName}, Quantity: {notification.Quantity}, OccurredAt: {notification.OccurredAt}");
            return Task.CompletedTask;
        }
    }
}