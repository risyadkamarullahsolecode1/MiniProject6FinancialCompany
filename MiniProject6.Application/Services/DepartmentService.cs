using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MiniProject6.Application.Dtos;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using MiniProject6.Application.Dtos.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Services
{
    public class DepartmentService:IDepartmentService
    {
        private readonly IDepartmentRepository _departmentrepository;
        private readonly IEmployeeRepository _employeerepository;
        private IProjectRepository _projectrepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public readonly IWorksonRepository _worksonrepository;

        public DepartmentService(IDepartmentRepository departmentrepository, IEmployeeRepository employeeRepository, IProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IWorksonRepository worksonrepository)
        {
            _departmentrepository = departmentrepository;
            _employeerepository = employeeRepository;
            _projectrepository = projectRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _worksonrepository = worksonrepository;
        }

        public async Task<Employee> AssignEmployeeToDepartment(int empNo, int deptNo)
        {
            var employee = await _employeerepository.GetEmployeeById(empNo);
            if (employee == null) 
            {
                throw new ArgumentException("Employee not found");
            }

            await _employeerepository.UpdateEmployee(employee);
            await _employeerepository.SaveChangesAsync();
            return employee;
        }
        public async Task<EmployeeDetails> GetEmployeesUnderSupervisorAsync(int spvEmpNo)
        {
            var employee = await _employeerepository.GetEmployeeById(spvEmpNo);
            if(employee == null) throw new KeyNotFoundException($"Supervisor with No {spvEmpNo} not found.");

            var department = await _departmentrepository.GetDepartmentById(employee.Deptno.Value);
            if (department == null) 
            { 
                throw new KeyNotFoundException($"Department with No {employee.Deptno} not found.");
            }
            Employee supervisor = null;
            if (department.Spvempno.HasValue)
            {
                supervisor = await _employeerepository.GetEmployeeById(spvEmpNo);
            }
            var supervisorName = supervisor != null ? supervisor.Fname + " " + supervisor.Lname : "No Manager";


            var employeeDetailList = new EmployeeDetails
            {
                EmployeeName = employee.Fname + " " + employee.Lname,
                Address = employee.Address,
                PhoneNumber = employee.Phonenumber.HasValue ? employee.Phonenumber.Value.ToString() : "Not available",
                Email = employee.Email,
                Position = employee.Position,
                SupervisorName = supervisorName,
                EmployeeType = employee.Employeetype
            };
            employeeDetailList.ToString();

            return employeeDetailList;
        }

        public async Task<List<ProjectResponseDto>> GetProjectsForDepartmentAsync()
        {
            // Get the username of the currently logged-in user
            var currentUser = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Retrieve the logged-in user's details
            var currentUserObject = await _userManager.FindByNameAsync(currentUser);

            if (currentUserObject == null)
            {
                throw new Exception("Current user not found.");
            }

            // Fetch the employee record for the logged-in user using the UserId
            var loggedInEmployee = await _employeerepository.GetEmployeeByUserIdAsync(currentUserObject.Id);

            if (loggedInEmployee == null || !loggedInEmployee.Deptno.HasValue)
            {
                throw new Exception("Employee or department record not found for the logged-in user.");
            }

            // Check if the logged-in user is a Department Manager
            var currentUserRoles = _httpContextAccessor.HttpContext?.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (currentUserRoles == null || !currentUserRoles.Contains("Department Manager")) 
            {
                throw new UnauthorizedAccessException("Only Department Manager can view projects for the department.");
            }

            // Fetch projects in the same department
            var projectsInSameDepartment = await _projectrepository.GetProjectsByDepartmentAsync(loggedInEmployee.Deptno.Value);

            // Map the project records to a response DTO
            var projectResponseList = projectsInSameDepartment.Select(project => new ProjectResponseDto
            {
                ProjNo = project.Projno,
                ProjName = project.Projname,
                Deptno = project.Deptno,
                Location = project.LocationId,
            }).ToList();

            return projectResponseList;
        }

        public async Task UpdateProjectAssignmentsAsync(Workson workson)
        {
            // Get the username of the currently logged-in user
            var currentUser = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Retrieve the logged-in user's details
            var currentUserObject = await _userManager.FindByNameAsync(currentUser);

            if (currentUserObject == null)
            {
                throw new Exception("Current user not found.");
            }

            // Fetch the employee record for the logged-in user using the UserId
            var loggedInEmployee = await _employeerepository.GetEmployeeByUserIdAsync(currentUserObject.Id);

            if (loggedInEmployee == null || !loggedInEmployee.Deptno.HasValue)
            {
                throw new Exception("Employee or department record not found for the logged-in user.");
            }

            // Check if the logged-in user is an Employee Supervisor
            var currentUserRoles = _httpContextAccessor.HttpContext?.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (currentUserRoles == null || !currentUserRoles.Contains("Employee Supervisor"))
            {
                throw new UnauthorizedAccessException("Only Employee Supervisors can manage project assignments.");
            }

            // Check if the supervised employee belongs to the same department
            var supervisedEmployee = await _employeerepository.GetEmployeeById(workson.Empno);

            if (supervisedEmployee == null || supervisedEmployee.Deptno != loggedInEmployee.Deptno)
            {
                throw new Exception("The supervised employee does not belong to the same department.");
            }

            // Update the project assignment in the repository
            await _worksonrepository.AddWorkson(workson);
        }

    }
}
