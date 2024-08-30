using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {

            _locationRepository = locationRepository;
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetAllLocations()
        {
            var Location = await _locationRepository.GetAllLocations();
            return Ok(Location);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpPost]
        public async Task<ActionResult<Location>> AddLocation(Location location)
        {
            var createdLocation = await _locationRepository.AddLocation(location);
            return Ok(createdLocation);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpPut]
        public async Task<ActionResult<Location>> UpdateLocation(Location location)
        {
            var updatedLocation = await _locationRepository.UpdateLocation(location);
            return Ok(updatedLocation);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLocation(Location location)
        {
            await _locationRepository.DeleteLocation(location);
            return Ok("location has been deleted");
        }
    }
}
