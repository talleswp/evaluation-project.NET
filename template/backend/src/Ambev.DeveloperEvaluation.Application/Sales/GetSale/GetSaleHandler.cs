
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handles the query to get a sale by its ID.
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the <see cref="GetSaleQuery"/> to get a sale by its ID.
    /// </summary>
    /// <param name="query">The query to get a sale.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The sale details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the sale is not found.</exception>
    public async Task<GetSaleResult> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(query.Id, cancellationToken);

        if (sale == null)
        {
            throw new KeyNotFoundException($"Sale with ID {query.Id} not found.");
        }

        return _mapper.Map<GetSaleResult>(sale);
    }
}
