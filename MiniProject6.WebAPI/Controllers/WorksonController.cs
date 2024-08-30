using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class WorksonController : ControllerBase
    {
        private readonly IWorksonRepository _worksonRepository;
        public WorksonController(IWorksonRepository worksonRepository)
        {
            _worksonRepository = worksonRepository;
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee Supervisor, Department Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workson>>> GetAllWorkson()
        {
            var workson = await _worksonRepository.GetAllWorkson();
            return Ok(workson);
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee Supervisor, Department Manager")]
        [HttpGet("{empNo}/{projNo}")]
        public async Task<ActionResult<Workson>> GetProjectById(int empNo, int projNo)
        {
            var workson = await _worksonRepository.GetWorksonById(empNo, projNo);
            if (workson == null)
            {
                return NotFound();
            }
            return Ok(workson);
        }
        [Authorize(Roles = "Administrator, Department Manager, Employee")]
        [HttpGet("{empNo}")]
        public async Task<ActionResult<IEnumerable<Workson>>> GetWorksonByEmployee(int empNo)
        {
            var workson = await _worksonRepository.GetWorksonByEmployee(empNo);
            return Ok(workson);
        }
        [Authorize(Roles = "Administrator ")]
        [HttpPost]
        public async Task<ActionResult<Project>> AddDepartment(Workson workson)
        {
            var createdworkson = await _worksonRepository.AddWorkson(workson);
            return Ok(createdworkson);
        }
        [Authorize(Roles = "Administrator, Employee Supervisor")]
        [HttpPut("{empNo}/{projNo}")]
        public async Task<IActionResult> UpdateEmployee(int empNo, int projNo, Workson workson)
        {
            if (projNo != workson.Projno && empNo != workson.Empno) return BadRequest();

            var updatedworkson = await _worksonRepository.UpdateWorkson(workson);
            return Ok(updatedworkson);
        }
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{empNo}/{projNo}")]
        public async Task<ActionResult<bool>> DeleteWorkson(int empNo, int projNo)
        {
            var deleted = await _worksonRepository.DeleteWorkson(empNo, projNo);
            if (!deleted) return NotFound();
            return Ok("project has been deleted !");
        }
    }
}
