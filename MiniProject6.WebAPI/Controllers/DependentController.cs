using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DependentController : ControllerBase
    {
        private readonly IDependentRepository _dependentRepository;

        public DependentController(IDependentRepository dependentRepository)
        {
            _dependentRepository = dependentRepository;
        }

        [Authorize(Roles = "Administrator, HR Manager, Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dependent>>> GetAllDependant()
        {
            var dependant = await _dependentRepository.GetAllDependent();
            return Ok(dependant);
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee")]
        [Authorize(Roles = "Employee")]
        [HttpGet("{dependantno}")]
        public async Task<ActionResult<Dependent>> GetEmployeeById(int dependantno)
        {
            var dependant = await _dependentRepository.GetDependentById(dependantno);
            if (dependant == null)
            {
                return NotFound();
            }
            return Ok(dependant);
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee")]
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<Dependent>> AddDepartment(Dependent dependant)
        {
            var createddependant = await _dependentRepository.AddDependent(dependant);
            return Ok(createddependant);
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee")]
        [HttpPut("{dependantno}")]
        public async Task<IActionResult> UpdateEmployee(int dependantno, Dependent dependant)
        {
            if (dependantno != dependant.Dependentno) return BadRequest();

            var updatedDepartment = await _dependentRepository.UpdateDependent(dependant);
            return Ok(updatedDepartment);
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee")]
        [HttpDelete("{dependantno}")]
        public async Task<ActionResult<bool>> DeleteBook(int deptNo)
        {
            var deleted = await _dependentRepository.DeleteDependent(deptNo);
            if (!deleted) return NotFound();
            return Ok("department has been deleted !");
        }
    }
}
