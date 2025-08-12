using MediatR;
using Serilog;
using Ambev.DeveloperEvaluation.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
    {
        public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            Log.Information($"Domain Event: SaleCreatedEvent - SaleId: {notification.SaleId}, SaleNumber: {notification.SaleNumber}, OccurredAt: {notification.OccurredAt}");
            return Task.CompletedTask;
        }
    }
}