﻿using DogHouse.Domain.Entities;

namespace DogHouse.Infrastructure.Interfaces
{
    public interface IDogRepository
    {
        Task<IEnumerable<Dog>> GetAllDogsAsync(Dictionary<string, string> attributes, int pageNumber, int pageSize);

        Task<Dog?> GetDogByNameAsync(string name);

        Task<Dog> AddDogAsync(Dog dog);

        Task<bool> DogExistsAsync(string name);
    }
}