using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Application.Repositories;
using DogHouse.Domain.DTOs;
using DogHouse.Domain.Entities;
using DogHouse.Infrastructure.Interfaces;
using FluentValidation;

namespace DogHouse.Application.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository dogRepository;
        private readonly IMapper mapper;
        private readonly IValidator<DogDto> validator;

        public DogService(IDogRepository dogRepository, IMapper mapper, IValidator<DogDto> validator)
        {
            this.dogRepository = dogRepository;
            this.mapper = mapper;
            this.validator = validator;
        }

        public async Task<Result<DogDto>> CreateDogAsync(DogDto dogDto)
        {
            var validationResult = await validator.ValidateAsync(dogDto);
            if (!validationResult.IsValid)
            {
                return Result<DogDto>.Invalid(validationResult.AsErrors());
            }
            if (await dogRepository.DogExistsAsync(dogDto.Name))
            {
                return Result<DogDto>.Error("Dog with the same name already exists.");
            }
            var dog = mapper.Map<Dog>(dogDto);
            var newDog = await dogRepository.AddDogAsync(dog);
            return mapper.Map<DogDto>(newDog); ;
        }

        public async Task<Result<List<DogDto>>> GetDogsAsync(Dictionary<string, string> attributes, int pageNumber, int pageSize)
        {
            var dogs = await dogRepository.GetAllDogsAsync(attributes, pageNumber, pageSize);
            var dogDtos = mapper.Map<List<DogDto>>(dogs);
            return Result<List<DogDto>>.Success(dogDtos);
        }
    }
}