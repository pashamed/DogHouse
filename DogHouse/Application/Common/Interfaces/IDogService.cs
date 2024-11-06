using Ardalis.Result;
using DogHouse.Domain.DTOs;

namespace DogHouse.Application.Common.Interfaces
{
    public interface IDogService
    {
        Task<Result<IReadOnlyList<DogDto>>> GetDogsAsync(DogFitlerDto filter, int pageNumber, int pageSize);

        Task<Result<DogDto>> CreateDogAsync(DogDto dogDto);
    }
}