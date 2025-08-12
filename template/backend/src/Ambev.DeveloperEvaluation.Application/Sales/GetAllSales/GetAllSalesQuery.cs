
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesQuery : IRequest<GetAllSalesResult>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; } = "SaleDate";
    public string SortOrder { get; set; } = "desc";
    public string? Customer { get; set; }
    public string? Branch { get; set; }
    public bool? IsCancelled { get; set; }
}
