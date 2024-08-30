using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        [Authorize(Roles = "Administrator, Department Manager, Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProject()
        {
            var project = await _projectRepository.GetAllProject();
            return Ok(project);
        }
        [Authorize(Roles = "Administrator, Department Manager, Employee")]
        [HttpGet("{projNo}")]
        public async Task<ActionResult<Project>> GetProjectById(int projNo)
        {
            var project = await _projectRepository.GetProjectById(projNo);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpPost]
        public async Task<ActionResult<Project>> AddDepartment(Project project)
        {
            var createdproject = await _projectRepository.AddProject(project);
            return Ok(createdproject);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpPut("{projNo}")]
        public async Task<IActionResult> UpdateEmployee(int projNo, Project project)
        {
            if (projNo != project.Projno) return BadRequest();

            var updatedproject = await _projectRepository.UpdateProject(project);
            return Ok(updatedproject);
        }
        [Authorize(Roles = "Administrator, Department Manager")]
        [HttpDelete("{projNo}")]
        public async Task<ActionResult<bool>> DeleteBook(int projNo)
        {
            var deleted = await _projectRepository.DeleteProject(projNo);
            if (!deleted) return NotFound();
            return Ok("project has been deleted !");
        }
    }
}
