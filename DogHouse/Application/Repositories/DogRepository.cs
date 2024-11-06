using DogHouse.Domain.Entities;
using DogHouse.Infrastructure;
using DogHouse.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DogHouse.Application.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly AppDbContext _context;

        public DogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dog> AddDogAsync(Dog dog)
        {
            await _context.Dogs.AddAsync(dog);
            await _context.SaveChangesAsync();
            return dog;
        }

        public async Task<bool> DogExistsAsync(string name)
        {
            return await _context.Dogs.AnyAsync(d => d.Name == name);
        }

        public async Task<IEnumerable<Dog>> GetAllDogsAsync(Dictionary<string, string> attributes, int pageNumber, int pageSize)
        {
            var query = _context.Dogs.AsQueryable();

            if (attributes != null && attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    query = attribute.Key.ToLower() switch
                    {
                        "name" => attribute.Value == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                        "color" => attribute.Value == "desc" ? query.OrderByDescending(d => d.Colors) : query.OrderBy(d => d.Colors),
                        "tail_length" => attribute.Value == "desc" ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength),
                        "weight" => attribute.Value == "desc" ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight),
                        _ => query
                    };
                }
            }

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Dog?> GetDogByNameAsync(string name)
        {
            return await _context.Dogs.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}