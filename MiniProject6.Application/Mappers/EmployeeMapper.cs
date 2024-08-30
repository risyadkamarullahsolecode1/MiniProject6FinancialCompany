using MiniProject6.Application.Dtos;
using MiniProject6.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToEmployeeDto(this Employee employee)
        {
            return new EmployeeDto
            {
                empNo = employee.Empno,
                EmployeeName = employee.Fname+" "+employee.Lname,
                Dob = employee.Dob,
                Address = employee.Address,
                Phonenumber = employee.Phonenumber,
                Email = employee.Email,
                Deptno = employee.Deptno,
                Position = employee.Position,
                Employeetype = employee.Employeetype,
                Level = employee.Level,
                Salary = employee.Salary,
                Nik = employee.Nik,
                Lastupdateddate = employee.Lastupdateddate,
                Status = employee.Status,
                StatusReason = employee.Statusreason
            };
        }
    }
}
