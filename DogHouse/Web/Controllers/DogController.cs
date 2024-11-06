using DogHouse.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DogHouse.Web.Controllers
{
    [ApiController]
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
    }
}
