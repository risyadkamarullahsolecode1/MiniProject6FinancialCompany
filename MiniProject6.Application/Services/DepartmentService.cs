using MiniProject6.Application.Dtos;
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
    public class DepartmentService:IDepartmentService
    {
        private readonly IDepartmentRepository _departmentrepository;
        private readonly IEmployeeRepository _employeerepository;

        public DepartmentService(IDepartmentRepository departmentrepository, IEmployeeRepository employeeRepository)
        {
            _departmentrepository = departmentrepository;
            _employeerepository = employeeRepository;
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
    }
}
