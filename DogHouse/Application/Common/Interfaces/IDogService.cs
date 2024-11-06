using Ardalis.Result;
using DogHouse.Domain.DTOs;

namespace DogHouse.Application.Common.Interfaces
{
    public interface IDogService
    {
        Task<Result<List<DogDto>>> GetDogsAsync(Dictionary<string, string> attributes, int pageNumber, int pageSize);

        Task<Result<DogDto>> CreateDogAsync(DogDto dogDto);
    }
}