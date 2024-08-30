using MiniProject6.Application.Dtos;
using MiniProject6.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<Employee> AssignEmployeeToDepartment(int empNo, int deptNo);
        Task<EmployeeDetails> GetEmployeesUnderSupervisorAsync(int spvEmpNo);
    }
}
