using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniProject6.Application.Dtos;
using MiniProject6.Application.Dtos.Account;
using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using MiniProject6.Application.Dtos.Dependent;
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
        private readonly IDependentRepository _dependentRepository;
        private readonly IWorksonRepository _worksonRepository;
        public EmployeeService(IEmployeeRepository employeeRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IDepartmentRepository departmentRepository, IDependentRepository dependentRepository, IWorksonRepository worksonRepository)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _departmentRepository = departmentRepository;
            _dependentRepository = dependentRepository;
            _worksonRepository = worksonRepository;
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
                Lastupdateddate = DateTime.UtcNow,
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
                Lastupdateddate = DateTime.UtcNow,
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
                EmpNo = e.Empno,
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
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (username == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var employees = await _departmentRepository.GetEmployeesBySupervisorIdAsync(spvEmpNo);
            return employees.Select(e => new EmployeeDto
            {
                EmpNo = e.Empno,
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

        public async Task<object> GetFilteredSortedEmployeesAsync(SearchDto searchDto)
        {
            // Retrieve employee and department data as queryable
            var employees = _employeeRepository.GetAllEmployee().AsQueryable();
            var departments = _departmentRepository.GetAllDepartment().AsQueryable();

            // Apply keyword filtering across all relevant fields
            if (!string.IsNullOrEmpty(searchDto.Keyword))
            {
                employees = employees.Where(e =>
                    e.Fname.ToLower().Contains(searchDto.Keyword.ToLower()) ||
                    e.Lname.ToLower().Contains(searchDto.Keyword.ToLower()) ||
                    e.Position.ToLower().Contains(searchDto.Keyword.ToLower()) ||
                    e.Employeetype.ToLower().Contains(searchDto.Keyword.ToLower())
                );

                departments = departments.Where(d =>
                    d.Deptname.ToLower().Contains(searchDto.Keyword.ToLower())
                );
            }

            // Apply specific filters
            if (!string.IsNullOrEmpty(searchDto.EmployeeName))
                employees = employees.Where(e =>
                    (e.Fname + " " + e.Lname).ToLower().Contains(searchDto.EmployeeName.ToLower()));

            if (!string.IsNullOrEmpty(searchDto.Position))
                employees = employees.Where(e => e.Position.ToLower().Contains(searchDto.Position.ToLower()));

            if (searchDto.Level > 0)
                employees = employees.Where(e => e.Level == searchDto.Level);

            if (!string.IsNullOrEmpty(searchDto.EmploymentType))
                employees = employees.Where(e => e.Employeetype.ToLower().Equals(searchDto.EmploymentType.ToLower()));

            if (!string.IsNullOrEmpty(searchDto.DepartmentName))
                departments = departments.Where(d => d.Deptname.ToLower().Contains(searchDto.DepartmentName.ToLower()));

            employees = employees.Where(e => e.Deptno != null);
            departments = departments.Where(d => d.Deptno != null);

            var query = from e in employees
                        join d in departments on e.Deptno equals d.Deptno into deptGroup
                        from dept in deptGroup.DefaultIfEmpty()
                        select new EmployeeSortedDto
                        {
                            EmpNo = e.Empno,
                            Fname = e.Fname + " " + e.Lname,
                            Deptname = dept.Deptname ?? "No Department",
                            Position = e.Position,
                            Level = e.Level,
                            Employeetype = e.Employeetype,
                            Lastupdateddate = e.Lastupdateddate
                        };


            // Apply sorting
            if (!string.IsNullOrEmpty(searchDto.SortBy))
            {
                query = searchDto.SortBy.ToLower() switch
                {
                    "name" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Fname) :
                        query.OrderBy(e => e.Fname),
                    "department" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Deptname) :
                        query.OrderBy(e => e.Deptname),
                    "position" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Position) :
                        query.OrderBy(e => e.Position),
                    "level" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Level) :
                        query.OrderBy(e => e.Level),
                    "employmenttype" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Employeetype) :
                        query.OrderBy(e => e.Employeetype),
                    "lastupdateddate" => searchDto.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase) ?
                        query.OrderByDescending(e => e.Lastupdateddate) :
                        query.OrderBy(e => e.Lastupdateddate),
                    _ => query.OrderBy(e => e.EmpNo)
                };
            }

            // Get total record count before pagination
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
            var paginatedEmployees = await query.Skip(skip).Take(searchDto.PageSize).ToListAsync();

            // Return the paginated data and total record count
            return new { total = totalRecords, data = paginatedEmployees };
        }

        //Deactive Employee 
        public async Task DeactivateEmployeeAsync(int empNo, string reason)
        {
            var employee = await _employeeRepository.GetEmployeeById(empNo);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with No {empNo} not found.");
            }

            employee.Status = "Not Active";
            employee.Statusreason = reason;
            employee.Lastupdateddate = DateTime.Now;

            await _employeeRepository.UpdateEmployee(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        // update employee with timestamp
        public async Task UpdateEmployeeAsync(int empNo, UpdateDto updateDto)
        {
            var employee = await _employeeRepository.GetEmployeeById(empNo);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with No {empNo} not found.");
            }

            employee.Fname = updateDto.Fname ?? employee.Fname;
            employee.Lname = updateDto.Lname ?? employee.Lname;
            employee.Address = updateDto.Address ?? employee.Address;
            employee.Phonenumber = updateDto.Phonenumber ?? employee.Phonenumber;
            employee.Email = updateDto.Email ?? employee.Email;
            employee.Position = updateDto.Position ?? employee.Position;
            employee.Deptno = updateDto.Deptno ?? employee.Deptno;
            employee.Employeetype = updateDto.Employeetype ?? employee.Employeetype;
            employee.Statusreason = updateDto.Statusreason ?? employee.Statusreason;
            employee.Status = updateDto.Status ?? employee.Status;
            employee.Nik = updateDto.Nik ?? employee.Nik;
            employee.Lastupdateddate = DateTime.Now; // Update timestamp

            await _employeeRepository.UpdateEmployee(employee);
            await _employeeRepository.SaveChangesAsync();

            // Map to EmployeeDto for returning
            var employeeDto = new EmployeeDto
            {
                Fname = employee.Fname,
                Lname = employee.Lname,
                Address = employee.Address,
                Email = employee.Email,
                Position = employee.Position,
                Phonenumber = employee.Phonenumber,
                Employeetype = employee.Employeetype,
                Lastupdateddate = employee.Lastupdateddate
            };

            return;
        }

        public async Task<EmployeeWithDepartmentDto> GetEmployeeWithDepartmentByIdAsync(int empNo)
        {
            // Fetch the employee by empNo
            var employee = await _employeeRepository.GetEmployeeById(empNo);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with No {empNo} not found.");
            }

            // Fetch the department using the deptNo from the employee
            var department = await _departmentRepository.GetDepartmentById(employee.Deptno.Value);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with No {employee.Deptno} not found.");
            }

            // Fetch the supervisor employee using SpvEmpNo from the department (if it's not null)
            string spvEmpName = null;
            if (department.Spvempno.HasValue)
            {
                var supervisor = await _employeeRepository.GetEmployeeById(department.Spvempno.Value);
                if (supervisor != null)
                {
                    spvEmpName = $"{supervisor.Fname} {supervisor.Lname}";  // Full name of the supervisor
                }
            }

            // Create a DTO and populate it with employee and department data
            var employeeWithDeptDto = new EmployeeWithDepartmentDto
            {
                Empno = employee.Empno,
                Fname = employee.Fname,
                Lname = employee.Lname,
                Position = employee.Position,
                DeptName = department.Deptname,
                Dob = employee.Dob,
                Address = employee.Address,
                Sex = employee.Sex,
                Deptno = employee.Deptno,
                Employeetype = employee.Employeetype,
                Level = employee.Level,
                Lastupdateddate = employee.Lastupdateddate,
                Nik = employee.Nik,
                Status = employee.Status,
                Statusreason = employee.Statusreason,
                Salary = employee.Salary,
                SpvEmpName = spvEmpName,
            };

            return employeeWithDeptDto;
        }

        public async Task AddDependentAsync(int empNo, DependentDto dependentDto)
        {
            // Check if the employee exists
            var employee = await _employeeRepository.GetEmployeeById(empNo);

            if (employee == null)
            {
                throw new ArgumentException($"Employee with empNo {empNo} does not exist.");
            }

            // Create a new Dependent entity and map data from the DTO
            var dependent = new Dependent
            {
                Empno = empNo,
                Name = dependentDto.Name,
                Sex = dependentDto.Sex,
                Relationship = dependentDto.Relationship,
                Dob = dependentDto.Dob
            };

            // Add the dependent to the database
            await _dependentRepository.AddDependent(dependent);
            await _dependentRepository.SaveChangesAsync();
        }

        public async Task AddDependentLoginAsync(DependentDto dependentDto)
        {

            // Get the username of the currently logged-in user
            var userName = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (userName == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Retrieve employee information from the Employee table using UserId
            var employee = await _employeeRepository.GetEmployeeByUserId(user.Id);

            if (employee == null)
            {
                throw new Exception("Employee information not found.");
            }
            // Check if the employee exists
            var employees = await _employeeRepository.GetEmployeeById(employee.Empno);

            if (employee == null)
            {
                throw new ArgumentException($"Employee with empNo {employee.Empno} does not exist.");
            }

            // Create a new Dependent entity and map data from the DTO
            var dependent = new Dependent
            {
                Empno = employees.Empno,
                Name = dependentDto.Name,
                Sex = dependentDto.Sex,
                Relationship = dependentDto.Relationship,
                Dob = dependentDto.Dob
            };

            // Add the dependent to the database
            await _dependentRepository.AddDependent(dependent);
            await _dependentRepository.SaveChangesAsync();
        }

        public async Task<List<EmployeeDepartmentDto>> GetEmployeesInSameDepartmentAsync()
        {
            // Get the username of the currently logged-in user
            var currentUser = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Retrieve the logged-in user's details (via UserManager)
            var currentUserObject = await _userManager.FindByNameAsync(currentUser);

            if (currentUserObject == null)
            {
                throw new Exception("Current user not found.");
            }

            // Fetch the employee record for the logged-in user using the UserId
            var loggedInEmployee = await _employeeRepository.GetEmployeeByUserIdAsync(currentUserObject.Id);

            if (loggedInEmployee == null)
            {
                throw new Exception("Employee record not found for the logged-in user.");
            }

            if (!loggedInEmployee.Deptno.HasValue)
            {
                return new List<EmployeeDepartmentDto>(); // No employees to return
            }

            // Fetch all employees in the same department
            var employeesInSameDepartment = await _employeeRepository.GetAllEmployeesInDepartmentAsync(loggedInEmployee.Deptno.Value);

            // Map the employee records to a response DTO
            var employeeResponseList = employeesInSameDepartment.Select(employee => new EmployeeDepartmentDto
            {
                EmpNo = employee.Empno,
                FirstName = employee.Fname,
                LastName = employee.Lname,
                Email = employee.Email, // Assuming Email is available in the Employee table
                Deptno = employee.Deptno,
                Position = employee.Position,
            }).ToList();

            return employeeResponseList;
        }

        public async Task<object> GetEmployeeDetails()
        {
            // Get the username of the currently logged-in user
            var userName = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (userName == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Retrieve employee information from the Employee table using UserId
            var employee = await _employeeRepository.GetEmployeeByUserId(user.Id);

            if (employee == null)
            {
                throw new Exception("Employee information not found.");
            }

            var dependents = await _dependentRepository.GetDependentsByEmpNo(employee.Empno);
            var workson = await _worksonRepository.GetWorksonByEmpNo(employee.Empno);
            // Fetch department details using DeptNo
            var department = await _departmentRepository.GetDepartmentById(employee.Deptno.Value);
            // Return the employee details along with dependents
            return new
            {
                EmployeeId = employee.Empno,
                FirstName = employee.Fname,
                LastName = employee.Lname,
                Address = employee.Address,
                Dob = employee.Dob,
                Sex = employee.Sex,
                Position = employee.Position,
                DepartmentId = employee.Deptno,
                PhoneNumber = employee.Phonenumber,
                Deptno = employee.Deptno,
                Status = employee.Status,
                Email = user.Email, // Retrieved from AspNetUser
                Assignments = workson.Select(workson => new
                {
                   ProjectNo = workson.Projno,
                   EmpNo = workson.Empno,
                   TotalHours = workson.Hoursworked,
                   DateWorked = workson.Dateworked,
                }).ToList(),
                Dependents = dependents.Select(dependent => new
                {
                    DependentId = dependent.Dependentno,
                    Name = dependent.Name,
                    Relationship = dependent.Relationship,
                    Dob = dependent.Dob
                }).ToList(),
                Department = department != null
                ? new
                {
                    DepartmentId = department.Deptno,
                    DepartmentName = department.Deptname,
                    ManagerId = department.Mgrempno,
                    SupervisorId = department.Spvempno
                }
                : null // Handle case where department might not exist
            };
        }

        /**public async Task<List<DependentInEmployee>> GetEmployeeWithDependent()
        {
            // Get the username of the currently logged-in user
            var currentUser = _httpContextAccessor.HttpContext?.User.Identity!.Name;

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User is not logged in.");
            }

            // Retrieve the logged-in user's details (via UserManager)
            var currentUserObject = await _userManager.FindByNameAsync(currentUser);

            if (currentUserObject == null)
            {
                throw new Exception("Current user not found.");
            }

            // Fetch the employee record for the logged-in user using the UserId
            var Employee = await _employeeRepository.GetEmployeeByUserIdAsync(currentUserObject.Id);

            if (Employee == null)
            {
                throw new Exception("Employee record not found for the logged-in user.");
            }

            // Fetch all employees in the same department
            var dependent = await _dependentRepository.GetDependentsByEmployeeAsync(Employee.Empno);

            // Map the employee records to a response DTO
            var employeeResponseList = dependent.Select(Employee => new DependentInEmployee
            {
                DependentName = Employee.,
                LastName = employee.Lname,
                Email = employee.Email, // Assuming Email is available in the Employee table
                Deptno = employee.Deptno,
                Position = employee.Position,
            }).ToList();

            return employeeResponseList;
        }**/
    }
}
