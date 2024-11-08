﻿using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using DogHouse.Application.Common;
using DogHouse.Application.Common.Interfaces;
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
            try
            {
                var dog = mapper.Map<Dog>(dogDto);
                var newDog = await dogRepository.AddDogAsync(dog);
                return Result.Created(mapper.Map<DogDto>(newDog));
            }
            catch (Exception ex)
            {
                return Result.Error($"An error occurred while creating the dog: {ex.Message}");
            }
        }

        public async Task<Result<IReadOnlyList<DogDto>>> GetDogsAsync(DogFitlerDto filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return Result.Error("Page number and page size must be greater than 0");
            }
            try
            {
                var dogs = await dogRepository.GetAllDogsAsync(filter);
                var dogDtos = mapper.Map<List<DogDto>>(dogs);
                return dogDtos.AsReadOnly();
            }
            catch (Exception ex)
            {
                return Result.Error($"An error occurred while getting dogs: {ex.Message}");
            }
        }
    }
}