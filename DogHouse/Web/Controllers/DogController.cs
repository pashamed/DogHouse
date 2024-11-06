using Ardalis.Result.AspNetCore;
using DogHouse.Application.Common;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Application.Services;
using DogHouse.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DogHouse.Web.Controllers
{
    [ApiController]
    [Route("/")]
    public class DogController : ControllerBase
    {
        private readonly IDogService dogService;

        public DogController(IDogService dogService)
        {
            this.dogService = dogService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Dogshouseservice.Version1.0.1");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<DogDto>>> GetDogs([FromQuery] string attribute, [FromQuery] string order, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var attributes = attribute?.Split(',').ToList() ?? new List<string>();
            var orders = order?.Split(',').ToList() ?? new List<string>();

            var filter = new DogFitlerDto
            {
                Attributes = attributes,
                Orders = orders
            };
            var result = await dogService.GetDogsAsync(filter, pageNumber, pageSize);
            if (result.IsSuccess)
            {
                return result.ToActionResult(this);
            }
            return BadRequest(result.Errors);
        }
    }
}
