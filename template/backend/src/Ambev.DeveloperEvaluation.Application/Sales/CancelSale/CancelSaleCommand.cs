
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public record CancelSaleCommand : IRequest<CancelSaleResult>
{
    public Guid Id { get; init; }

    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
