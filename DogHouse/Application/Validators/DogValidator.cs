using DogHouse.Domain.DTOs;
using DogHouse.Infrastructure.Interfaces;
using FluentValidation;

namespace DogHouse.Application.Validators
{
    public class DogValidator : AbstractValidator<DogDto>
    {
        public DogValidator(IDogRepository dogRepository)
        {
            RuleFor(dog => dog.Name)
                .MustAsync(async (name, cancellation) => !await dogRepository.DogExistsAsync(name)).WithMessage("Dog with the same name already exists")
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(dog => dog.Colors)
                .NotEmpty().WithMessage("Color is required.")
                .MaximumLength(50).WithMessage("Color must not exceed 50 characters.");

            RuleFor(dog => dog.TailLength)
                .GreaterThan(0).WithMessage("Tail length must be greater than 0.");

            RuleFor(dog => dog.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");
        }
    }
}