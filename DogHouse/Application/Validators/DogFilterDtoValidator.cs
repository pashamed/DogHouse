using DogHouse.Application.Common;
using FluentValidation;

namespace DogHouse.Application.Validators
{
    public class DogFilterDtoValidator : AbstractValidator<DogFitlerDto>
    {
        public DogFilterDtoValidator()
        {
            RuleFor(filter => filter.Attributes)
                .NotEmpty().WithMessage("Attributes are required.")
                .Must(attributes => attributes != null && attributes.Count > 0).WithMessage("Attributes must contain at least one item.");

            RuleFor(filter => filter.Orders)
                .NotEmpty().WithMessage("Orders are required.")
                .Must(orders => orders != null && orders.Count > 0).WithMessage("Orders must contain at least one item.");
        }
    }
}
