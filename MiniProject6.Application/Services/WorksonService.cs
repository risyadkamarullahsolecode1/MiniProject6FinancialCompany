using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Services
{
    public class WorksonService:IWorksonService
    {
        private readonly IWorksonRepository _worksonRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentrepository;
        private readonly IProjectRepository _projectrepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public WorksonService(IWorksonRepository worksonRepository, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _worksonRepository = worksonRepository;
            _employeeRepository = employeeRepository;
            _departmentrepository = departmentRepository;
            _projectrepository = projectRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Workson> AssignEmployeeWorkson(int empno, Workson workson)
        {
            var employee = await _employeeRepository.GetEmployeeById(empno);
            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found");
            }

            var workons = await _worksonRepository.UpdateWorkson(workson);
            return workons;
        }

        public Task<Workson> UpdateEmployeeWorkson(int empno, Workson workson)
        {
            throw new NotImplementedException();
        }

        public async Task AddProjectAssignmentsAsync(Workson workson)
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
            var loggedInEmployee = await _employeeRepository.GetEmployeeByUserIdAsync(currentUserObject.Id);

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
            var supervisedEmployee = await _employeeRepository.GetEmployeeById(workson.Empno);

            if (supervisedEmployee == null || supervisedEmployee.Deptno != loggedInEmployee.Deptno)
            {
                throw new Exception("The supervised employee does not belong to the same department.");
            }

            // Update the project assignment in the repository
            await _worksonRepository.AddWorkson(workson);
        }
    }
}
