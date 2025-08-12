
namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;

public class GetAllSalesRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "SaleDate";
    public string SortOrder { get; set; } = "desc";
    public string? Customer { get; set; }
    public string? Branch { get; set; }
    public bool? IsCancelled { get; set; }
}
