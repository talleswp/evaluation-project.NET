using MediatR;
using Serilog;
using Ambev.DeveloperEvaluation.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
    {
        public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
        {
            Log.Information($"Domain Event: SaleModifiedEvent - SaleId: {notification.SaleId}, SaleNumber: {notification.SaleNumber}, OccurredAt: {notification.OccurredAt}");
            return Task.CompletedTask;
        }
    }
}