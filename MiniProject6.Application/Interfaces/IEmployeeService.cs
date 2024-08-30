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
    }
}
