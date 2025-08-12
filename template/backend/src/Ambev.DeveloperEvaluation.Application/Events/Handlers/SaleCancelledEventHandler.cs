using MediatR;
using Serilog;
using Ambev.DeveloperEvaluation.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Events.Handlers
{
    public class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
    {
        public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
        {
            Log.Information($"Domain Event: SaleCancelledEvent - SaleId: {notification.SaleId}, SaleNumber: {notification.SaleNumber}, OccurredAt: {notification.OccurredAt}");
            return Task.CompletedTask;
        }
    }
}