using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniProject6.Application.Dtos;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using MiniProject6.Infrastructure.Data.Repository;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentRepository departmentRepository, IDepartmentService departmentService)
        {
            _departmentRepository = departmentRepository;
            _departmentService = departmentService;
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpGet]
        public ActionResult<IQueryable<Department>> GetAllDepartment()
        {
            var department = _departmentRepository.GetAllDepartment();
            return Ok(department);
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpGet("{deptNo}")]
        public async Task<ActionResult<Department>> GetEmployeeById(int deptNo)
        {
            var department = await _departmentRepository.GetDepartmentById(deptNo);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpPost]
        public async Task<ActionResult<Department>> AddDepartment(Department department)
        {
            var createdDepartment = await _departmentRepository.AddDepartment(department);
            return Ok(createdDepartment);
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpPut("{deptNo}")]
        public async Task<IActionResult> UpdateEmployee(int deptNo, Department department)
        {
            if (deptNo != department.Deptno) return BadRequest();

            var updatedDepartment = await _departmentRepository.UpdateDepartment(department);
            return Ok(updatedDepartment);
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpDelete("{deptNo}")]
        public async Task<ActionResult<bool>> DeleteBook(int deptNo)
        {
            var deleted = await _departmentRepository.DeleteDepartment(deptNo);
            if (!deleted) return NotFound();
            return Ok("department has been deleted !");
        }
        [Authorize(Roles = "Administrator, HR Department, Department Manager")]
        [HttpGet("manager/{deptNo}")]
        public async Task<IActionResult> GetManagerByDeptNo(int deptNo)
        {
            var manager = await _departmentRepository.GetManagerByDeptNoAsync(deptNo);
            if (manager == null)
            {
                return NotFound($"No manager found for department number {deptNo}");
            }
            return Ok(manager);
        }
        [Authorize(Roles = "Administrator, HR Manager")]
        [HttpPut("assign-employee-department")]
        public async Task<IActionResult> AssignEmployeeToDepartment(int empNo, int deptNo)
        {
            var result = await _departmentService.AssignEmployeeToDepartment(empNo, deptNo);
            return Ok(result);
        }
        [HttpGet("Supervisor")]
        public async Task<ActionResult<Employee>> GetSupervisorByDeptNoAsync(int deptNo)
        {
            var result = await _departmentRepository.GetSupervisorByDeptNoAsync(deptNo);
            return Ok(result);
        }

        [HttpGet("supervisor/{supervisorEmpNo}")]
        public async Task<IActionResult> GetEmployeesUnderSupervisorAsync(int spvEmpNo)
        {
            try
            {
                var employees = await _departmentService.GetEmployeesUnderSupervisorAsync(spvEmpNo);
                return Ok(employees);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("get-employee-by-supervisor")]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesUnderSupervisor(int spvEmpNo)
        {
            var result = await _departmentService.GetEmployeesUnderSupervisorAsync(spvEmpNo);
            return Ok(result);
        }
    }
}
