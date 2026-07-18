using FluentValidation;
using WebApp.Api.DTOs;

namespace WebApp.Api.DTOs
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(1000000).WithMessage("Price cannot exceed) 1,000,000.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
                .LessThanOrEqualTo(int.MaxValue).WithMessage($"Stock cannot exceed {int.MaxValue}.");
        }
    }
}
