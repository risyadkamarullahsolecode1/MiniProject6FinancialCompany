using MiniProject6.Application.Dtos;
using MiniProject6.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<ResponseModel> RegistrationEmployee(RegisterEmployee registerEmployee);
        Task<ResponseModel> UpdateRegistrationEmployee(int empno, RegisterEmployee registerEmployee);
        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDetailMaster> GetEmployeeByIdAsync(int empno);
        Task<List<EmployeeDto>> GetEmployeesUnderSupervisorAsync(int spvEmpNo);
        Task<object> GetFilteredSortedEmployeesAsync(SearchDto searchDto);
        Task DeactivateEmployeeAsync(int empNo, string reason);
        Task UpdateEmployeeAsync(int empNo, UpdateDto updateDto);
        Task<EmployeeWithDepartmentDto> GetEmployeeWithDepartmentByIdAsync(int empNo);
        Task AddDependentAsync(int empNo, DependentDto dependentDto);
        Task AddDependentLoginAsync(DependentDto dependentDto);
        Task<List<EmployeeDepartmentDto>> GetEmployeesInSameDepartmentAsync();
        Task<object> GetEmployeeDetails();
    }
}
