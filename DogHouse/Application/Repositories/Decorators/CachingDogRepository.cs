using DogHouse.Application.Common;
using DogHouse.Domain.Entities;
using DogHouse.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DogHouse.Application.Repositories.Decorators
{
    public class CachingDogRepository : IDogRepository
    {
        private readonly IDogRepository _inner;
        private readonly IMemoryCache _cacheService;

        public CachingDogRepository(IDogRepository inner, IMemoryCache cacheService)
        {
            _inner = inner;
            _cacheService = cacheService;
        }
        public async Task<Dog> AddDogAsync(Dog dog)
        {
            var result = await _inner.AddDogAsync(dog);
            _cacheService.Remove("AllDogs");
            return result;
        }

        public async Task<bool> DogExistsAsync(string name)
        {
            return await _inner.DogExistsAsync(name); ;
        }

        public async Task<IEnumerable<Dog>> GetAllDogsAsync(DogFitlerDto filter, int pageNumber, int pageSize)
        {
            var cacheKey = $"AllDogs_{pageNumber}_{pageSize}_{string.Join(",", filter.Attributes ?? new List<string>())}_{string.Join(",", filter.Orders ?? new List<string>())}";
            if (!_cacheService.TryGetValue(cacheKey, out IEnumerable<Dog>? dogs))
            {
                dogs = await _inner.GetAllDogsAsync(filter, pageNumber, pageSize);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cacheService.Set(cacheKey, dogs, cacheEntryOptions);
            }
            return dogs ?? [];
        }

        public async Task<Dog?> GetDogByNameAsync(string name)
        {
            var cacheKey = $"Dog_{name}";
            if (!_cacheService.TryGetValue(cacheKey, out Dog? dog))
            {
                dog = await _inner.GetDogByNameAsync(name);
                if (dog != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                    _cacheService.Set(cacheKey, dog, cacheEntryOptions);
                }
            }
            return dog;
        }
    }
}
