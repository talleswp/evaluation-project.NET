
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesQueryValidator : AbstractValidator<GetAllSalesQuery>
{
    public GetAllSalesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

        RuleFor(x => x.SortBy)
            .Must(BeAValidSortByField).WithMessage("Invalid SortBy field.");

        RuleFor(x => x.SortOrder)
            .Must(BeAValidSortOrder).WithMessage("Invalid SortOrder. Must be 'asc' or 'desc'.");
    }

    private bool BeAValidSortByField(string sortBy)
    {
        // Add valid fields for sorting here
        return new[] { "SaleDate", "Customer", "TotalAmount", "Branch" }.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeAValidSortOrder(string sortOrder)
    {
        return new[] { "asc", "desc" }.Contains(sortOrder, StringComparer.OrdinalIgnoreCase);
    }
}
