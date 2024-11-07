using Ardalis.Result.AspNetCore;
using DogHouse.Application.Common;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DogHouse.Web.Controllers
{
    [ApiController]
    [Route("/")]
    [EnableRateLimiting("Fixed")]
    public class DogsController : ControllerBase
    {
        private readonly IDogService dogService;

        public DogsController(IDogService dogService)
        {
            this.dogService = dogService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }

        [HttpGet("dogs")]
        public async Task<IResult> GetDogs([FromQuery] string? attribute, [FromQuery] string? order, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var attributes = attribute?.Split(',').ToList() ?? new List<string>();
            var orders = order?.Split(',').ToList() ?? new List<string>();

            var filter = new DogFitlerDto
            {
                Attributes = attributes,
                Orders = orders
            };
            var result = await dogService.GetDogsAsync(filter, pageNumber, pageSize);
            return result.ToMinimalApiResult();
        }

        [HttpPost("dog")]
        public async Task<IResult> CreateDog([FromBody] DogDto dogDto)
        {
            var result = await dogService.CreateDogAsync(dogDto);
            return result.ToMinimalApiResult();
        }
    }
}