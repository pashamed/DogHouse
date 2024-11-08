﻿using DogHouse.Application.Common;
using DogHouse.Domain.Entities;

namespace DogHouse.Infrastructure.Interfaces
{
    public interface IDogRepository
    {
        Task<IEnumerable<Dog>> GetAllDogsAsync(DogFitlerDto filter);

        Task<Dog?> GetDogByNameAsync(string name);

        Task<Dog> AddDogAsync(Dog dog);

        Task<bool> DogExistsAsync(string name);
    }
}