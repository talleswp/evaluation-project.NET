
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.Customer)
            .NotEmpty().WithMessage("Customer name cannot be empty.")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(sale => sale.Branch)
            .NotEmpty().WithMessage("Branch cannot be empty.")
            .MaximumLength(50).WithMessage("Branch cannot exceed 50 characters.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.")
            .Must(items => items.All(item => new SaleItemValidator().Validate(item).IsValid))
            .WithMessage("One or more sale items are invalid.");
    }
}
