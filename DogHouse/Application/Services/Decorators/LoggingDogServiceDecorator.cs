using Ardalis.Result;
using DogHouse.Application.Common;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Domain.DTOs;

namespace DogHouse.Application.Services.Decorators
{
    public class LoggingDogServiceDecorator : IDogService
    {
        private readonly IDogService _inner;
        private readonly ILogger<LoggingDogServiceDecorator> _logger;
        public LoggingDogServiceDecorator(IDogService inner, ILogger<LoggingDogServiceDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }
        public async Task<Result<DogDto>> CreateDogAsync(DogDto dogDto)
        {
            _logger.LogInformation("Creating dog: {Name}", dogDto.Name);
            var result = await _inner.CreateDogAsync(dogDto);
            _logger.LogInformation("Dog created: {Name}", result.Value?.Name);
            return result;
        }

        public async Task<Result<IReadOnlyList<DogDto>>> GetDogsAsync(DogFitlerDto filter, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Getting dogs with filter: Attributes={Attributes}, Orders={Orders}, PageNumber={PageNumber}, PageSize={PageSize}",
                    string.Join(", ", filter.Attributes ?? new List<string>()),
                    string.Join(", ", filter.Orders ?? new List<string>()),
                    pageNumber,
                    pageSize);
            var result = await _inner.GetDogsAsync(filter, pageNumber, pageSize);
            _logger.LogInformation("Dogs retrieved: {Count}", result.Value?.Count);
            return result;
        }
    }
}
