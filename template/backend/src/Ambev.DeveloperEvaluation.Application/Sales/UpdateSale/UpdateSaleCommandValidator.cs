
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(x => x.Customer)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage("Branch is required.")
            .MaximumLength(50).WithMessage("Branch cannot exceed 50 characters.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required for a sale.")
            .Must(items => items.All(item => item.Quantity > 0 && item.UnitPrice > 0))
            .WithMessage("All sale items must have a quantity and unit price greater than zero.")
            .Must(items => items.All(item => item.Quantity <= 20))
            .WithMessage("Cannot sell more than 20 identical items.");

        RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}

public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required for sale item.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero for sale item.")
            .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20 for sale item.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero for sale item.");
    }
}
