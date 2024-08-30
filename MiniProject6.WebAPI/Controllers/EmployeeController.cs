using Microsoft.AspNetCore.Mvc;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using MiniProject6.Application.Mappers;
using MiniProject6.Application.Dtos;
using MiniProject6.Application.Dtos.Account;
using MiniProject6.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MiniProject6.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeRepository employeeRepository, IEmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }
        // get employee by id only for employee
        [Authorize(Roles = "Employee")]
        [HttpGet("employee/{empNo}")]
        public async Task<ActionResult<Employee>> GetEmployeeByIdEmployee(int empNo)
        {
            var employee = await _employeeRepository.GetEmployeeById(empNo);
            if (employee == null)
            {
                return NotFound();
            }
            var employeeDto = employee.ToEmployeeDto();
            return Ok(employee);
        }
        // get employee  only for employee
        [Authorize(Roles = "Employee")]
        [HttpGet]
        public ActionResult<IQueryable<Employee>> GetAllEmployee()
        {
            var employee = _employeeRepository.GetAllEmployee();
            var employeeDto = employee.Select(e => e.ToEmployeeDetailMaster()).ToList();
            return Ok(employeeDto);
        }
        // get employee for role except for employee
        [Authorize(Roles = "Administrator,HR Manager")]
        [HttpGet("view-employee")]
        public ActionResult<IQueryable<Employee>> GetAllEmployeeByAdmin()
        {
            var employee = _employeeRepository.GetAllEmployee();
            return Ok(employee);
        }
        // get employee by id for role except for employee
        [Authorize(Roles = "Administrator,HR Manager")]
        [HttpGet("{empNo}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int empNo)
        {
            var employee = await _employeeRepository.GetEmployeeById(empNo);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }
        // get employee for role except for employee
        [Authorize(Roles = "Administrator, HR Manager, Department Manager, Employee Supervisor")]
        [HttpGet("get-all-employee-details")]
        public async Task<ActionResult<List<EmployeeDto>>> GetAllEmployeesAsync()
        {
            try
            {
                var result = await _employeeService.GetAllEmployeesAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex) 
            { 
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Employee")]
        [HttpGet("get-employee-details")]
        public async Task<ActionResult<EmployeeDetailMaster>> GetAllEmployeesAsync(int empno)
        {
            try
            {
                var result = await _employeeService.GetEmployeeByIdAsync(empno);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // add employee for role except for employee
        [Authorize(Roles = "Administrator,HR Manager")]
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            var createdEmployee = await _employeeRepository.AddEmployee(employee);
            return Ok(createdEmployee);
        }

        // edit employee for role except for employee
        [Authorize(Roles = "Administrator,HR Manager")]
        [HttpPut("{empNo}")]
        public async Task<IActionResult> UpdateEmployee(int empNo, Employee employee)
        {
            if (empNo != employee.Empno) return BadRequest();

            var updatedEmployee = await _employeeRepository.UpdateEmployee(employee);
            var employeeDto = updatedEmployee;
            return Ok(employeeDto);
        }

        // get employee for role except for employee
        [HttpPut("update-by-employee/{empNo}")]
        public async Task<IActionResult> UpdateEmployeeAsync(int empNo, Employee employee)
        {
            if (empNo != employee.Empno) return BadRequest();

            var employeeEntity = employee.ToEmployeeDto();
            var updatedEmployee = await _employeeRepository.UpdateEmployeeAsync(employee);

            return Ok(updatedEmployee);
        }

        // delete employee for role except for employee
        [Authorize(Roles = "Administrator, HR Manager")]
        [HttpDelete("{empNo}")]
        public async Task<ActionResult<bool>> DeleteEmployee(int empNo)
        {
            var deleted = await _employeeRepository.DeleteEmployee(empNo);
            if (!deleted) return NotFound();
            return Ok("Employee has been deleted !");
        }

        //Add Employee and User only for several role
        [Authorize(Roles = "Administrator, HR Manager")]
        [HttpPost("Add-employee-user")]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployee registerEmployee)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _employeeService.RegistrationEmployee(registerEmployee);

            if (result.Status == "Error") return BadRequest(result.Message);
            return Ok(result);
        }

        // edit employee for role 
        [HttpPut("update-employee-user/{empno}")]
        public async Task<IActionResult> UpdateRegisterEmployee(int empno, RegisterEmployee registerEmployee)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _employeeService.UpdateRegistrationEmployee(empno, registerEmployee);

            if (result.Status == "Error") return BadRequest(result.Message);
            return Ok(result);
        }

        // get all employee below supervisor
        [Authorize(Roles = "Employee Supervisor")]
        [HttpGet("get-employee-supervisor")]
        public async Task<IActionResult> GetEmployeeUnderSupervisor(int spvEmpNo)
        {
            var result = await _employeeService.GetEmployeesUnderSupervisorAsync(spvEmpNo);
            return Ok(result);
        }
    }
}
