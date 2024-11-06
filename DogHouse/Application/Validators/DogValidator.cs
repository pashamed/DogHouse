using DogHouse.Domain.DTOs;
using FluentValidation;

namespace DogHouse.Application.Validators
{
    public class DogValidator : AbstractValidator<DogDto>
    {
        public DogValidator()
        {
            RuleFor(dog => dog.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(dog => dog.Color)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(50).WithMessage("Color must not exceed 50 characters.");

            RuleFor(dog => dog.TailLength)
                .GreaterThan(0).WithMessage("Tail length must be greater than 0.");

            RuleFor(dog => dog.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");
        }
    }
}