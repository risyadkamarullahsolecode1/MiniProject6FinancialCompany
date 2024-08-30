using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniProject6.Application.Dtos;
using MiniProject6.Application.Dtos.Account;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDepartmentRepository _departmentRepository;

        public EmployeeService(IEmployeeRepository employeeRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
        }
        public async Task<ResponseModel> RegistrationEmployee(RegisterEmployee registerEmployee)
        {
            var employee = await _employeeRepository.GetEmployeeById(registerEmployee.Empno);
            if (employee != null) return new ResponseModel { Status = "Error", Message = "User already exists!" };

            AppUser userApp = new AppUser()
            {
                Email = registerEmployee.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerEmployee.Fname + "" + registerEmployee.Lname
            };

            var result = await _userManager.CreateAsync(userApp, registerEmployee.Password);
            if (!result.Succeeded) return new ResponseModel
            {
                Status = "Error",
                Message = "User creation failed! Please check user details and try again."
            };

            var userId = userApp.Id;

            Employee employees = new Employee()
            {
                Empno = registerEmployee.Empno,
                Fname = registerEmployee.Fname,
                Lname = registerEmployee.Lname,
                Email = registerEmployee.Email,
                Address = registerEmployee.Address,
                Dob = registerEmployee.Dob,
                Sex = registerEmployee.Sex,
                Phonenumber = registerEmployee.Phonenumber,
                Position = registerEmployee.Position,
                Deptno = registerEmployee.Deptno,
                Employeetype = registerEmployee.Employeetype,
                Level = registerEmployee.Level,
                Lastupdateddate = registerEmployee.Lastupdateddate,
                Nik = registerEmployee.Nik,
                Salary = registerEmployee.Salary,
                Status = registerEmployee.Status,
                Statusreason = registerEmployee.Statusreason,
                UserId = userId,
            };
            await _employeeRepository.AddEmployee(employees);
            await _employeeRepository.SaveChangesAsync();
            return new ResponseModel { Status = "Success", Message = "Employee created succesfully!" };
        }
        public async Task<ResponseModel> UpdateRegistrationEmployee(int empno, RegisterEmployee registerEmployee)
        {
            var employee = await _employeeRepository.GetEmployeeById(registerEmployee.Empno);
            if (employee != null) return new ResponseModel { Status = "Error", Message = "User already exists!" };

            AppUser userApp = new AppUser()
            {
                Email = registerEmployee.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerEmployee.Fname + "" + registerEmployee.Lname
            };

            var result = await _userManager.UpdateAsync(userApp);
            if (!result.Succeeded) // Check for duplicate username error
                if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    return new ResponseModel
                    {
                        Status = "Error",
                        Message = "User creation failed! Username already exists."
                    };
                }

            Employee employees = new Employee()
            {
                Empno = registerEmployee.Empno,
                Fname = registerEmployee.Fname,
                Lname = registerEmployee.Lname,
                Email = registerEmployee.Email,
                Address = registerEmployee.Address,
                Dob = registerEmployee.Dob,
                Sex = registerEmployee.Sex,
                Phonenumber = registerEmployee.Phonenumber,
                Position = registerEmployee.Position,
                Deptno = registerEmployee.Deptno,
                Employeetype = registerEmployee.Employeetype,
                Level = registerEmployee.Level,
                Lastupdateddate = registerEmployee.Lastupdateddate,
                Nik = registerEmployee.Nik,
                Salary = registerEmployee.Salary,
                Status = registerEmployee.Status,
                Statusreason = registerEmployee.Statusreason
            };
            await _employeeRepository.UpdateEmployee(employees);
            await _employeeRepository.SaveChangesAsync();
            return new ResponseModel { Status = "Success", Message = "Employee updated succesfully!" };
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (username == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            if (username.Contains("Department Manager"))
            {
                throw new UnauthorizedAccessException("You do not have the required permissions.");
            }
            var employees = _employeeRepository.GetAllEmployee();
            return employees.Select(e => new EmployeeDto
            {
                empNo = e.Empno,
                EmployeeName = e.Fname + " " + e.Lname,
                Dob = e.Dob,
                Address = e.Address,
                Email = e.Email,
                Phonenumber = e.Phonenumber,
                Employeetype = e.Employeetype,
                Level = e.Level,
                Deptno = e.Deptno,
                Lastupdateddate = e.Lastupdateddate,
                Status = e.Status,
                StatusReason = e.Statusreason,
                Position = e.Position,
                Salary = e.Salary,
                Nik = e.Nik
            }).ToList();
        }

        public async Task<EmployeeDetailMaster> GetEmployeeByIdAsync(int empno)
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (username == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            
            var employees = await _employeeRepository.GetEmployeeById(empno);
            var employeeDetail = new EmployeeDetailMaster
            {
                empNo = employees.Empno,
                EmployeeName = employees.Fname+" "+employees.Lname,
                Email = employees.Email,
                Address = employees.Address,
                Dob = employees.Dob,
                Phonenumber = employees.Phonenumber,
                Position = employees.Position,
                Deptno = employees.Deptno,
                Employeetype = employees.Employeetype,
                Level = employees.Level,
                Lastupdateddate = employees.Lastupdateddate,
                Status = employees.Status,
                StatusReason = employees.Statusreason
            };

            return employeeDetail;
        }
        public async Task<List<EmployeeDto>> GetEmployeesUnderSupervisorAsync(int spvEmpNo)
        {
            var employees = await _departmentRepository.GetEmployeesBySupervisorIdAsync(spvEmpNo);
            return employees.Select(e => new EmployeeDto
            {
                empNo = e.Empno,
                EmployeeName= e.Fname+" "+e.Fname,
                Address = e.Address,
                Phonenumber= e.Phonenumber,
                Email= e.Email,
                Position = e.Position,
                Employeetype= e.Employeetype,
                Level = e.Level,
                Lastupdateddate = e.Lastupdateddate,
                Status = e.Status,
                StatusReason = e.Statusreason
            }).ToList();
        }
    }
}
