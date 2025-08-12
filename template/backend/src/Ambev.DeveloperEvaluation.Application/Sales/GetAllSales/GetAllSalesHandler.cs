
using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

/// <summary>
/// Handles the query to get all sales.
/// </summary>
public class GetAllSalesHandler : IRequestHandler<GetAllSalesQuery, GetAllSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetAllSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the <see cref="GetAllSalesQuery"/> to get all sales with pagination, sorting and filtering.
    /// </summary>
    /// <param name="query">The query to get all sales.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paginated list of sales.</returns>
    public async Task<GetAllSalesResult> Handle(GetAllSalesQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await _saleRepository.GetCountAsync(query.Customer, query.Branch, query.IsCancelled, cancellationToken);
        var sales = await _saleRepository.GetAllAsync(
            query.PageNumber, 
            query.PageSize, 
            query.SortBy, 
            query.SortOrder, 
            query.Customer, 
            query.Branch, 
            query.IsCancelled,
            cancellationToken
        );

        var mappedSales = _mapper.Map<IEnumerable<GetAllSalesItemResult>>(sales);

        return new GetAllSalesResult
        {
            Items = mappedSales,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }
}
