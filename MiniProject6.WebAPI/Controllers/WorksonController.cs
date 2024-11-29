using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using MiniProject6.Infrastructure.Data.Repository;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class WorksonController : ControllerBase
    {
        private readonly IWorksonRepository _worksonRepository;
        private readonly IWorksonService _worksonService;
        public WorksonController(IWorksonRepository worksonRepository, IWorksonService worksonService)
        {
            _worksonRepository = worksonRepository;
            _worksonService = worksonService;
        }
        [Authorize(Roles = "Administrator, HR Manager, Employee Supervisor, Department Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllWorkson()
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
            var workson = await _worksonRepository.GetWorksonByEmpNo(empNo);
            return Ok(workson);
        }
        [Authorize(Roles = "Administrator,Department Manager,Employee Supervisor")]
        [HttpPost]
        public async Task<ActionResult<Project>> AddDepartment(Workson workson)
        {
            var createdworkson = await _worksonRepository.AddWorkson(workson);
            return Ok(createdworkson);
        }
        [Authorize(Roles = "Administrator,Department Manager,Employee Supervisor")]
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
        [Authorize(Roles = "Administrator, Department Manager, Employee Supervisor")]
        [HttpPost("Assignment")]
        public async Task<IActionResult> UpdateAssignment([FromBody] Workson workson)
        {
            if (workson == null)
            {
                return BadRequest("Invalid workson object.");
            }

            try
            {
                await _worksonService.AddProjectAssignmentsAsync(workson);
                return Ok(new { Message = "Project assignment updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("{projNo}/projects")]
        public async Task<IActionResult> GetDepartmentAsync(int projNo)
        {
            var res = await _worksonRepository.GetEmployeesByProjectAsync(projNo);
            return Ok(res);
        }
    }
}
