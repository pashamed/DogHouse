using DogHouse.Application.Common;
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

        public async Task<IEnumerable<Dog>> GetAllDogsAsync(DogFitlerDto filter)
        {
            var query = _context.Dogs.AsQueryable();

            if (filter.Attributes != null && filter.Attributes.Count > 0)
            {
                for (int i = 0; i < filter.Attributes.Count; i++)
                {
                    string order = (filter.Orders != null && filter.Orders.Count > i) ? filter.Orders[i].ToLower() : "desc";

                    query = filter.Attributes[i].ToLower() switch
                    {
                        "name" => order == "desc" ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                        "color" => order == "desc" ? query.OrderByDescending(d => d.Colors) : query.OrderBy(d => d.Colors),
                        "tail_length" => order == "desc" ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength),
                        "weight" => order == "desc" ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight),
                        _ => query
                    };
                }
            }

            return await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        }

        public async Task<Dog?> GetDogByNameAsync(string name)
        {
            return await _context.Dogs.FirstOrDefaultAsync(d => d.Name == name);
        }
    }
}