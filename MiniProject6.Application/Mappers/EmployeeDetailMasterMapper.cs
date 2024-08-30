using MiniProject6.Application.Dtos;
using MiniProject6.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Mappers
{
    public static class EmployeeDetailMasterMapper
    {
        public static EmployeeDetailMaster ToEmployeeDetailMaster(this Employee employee)
        {
            return new EmployeeDetailMaster
            {
                empNo = employee.Empno,
                EmployeeName = employee.Fname + " " + employee.Lname,
                Address = employee.Address,
                Dob = employee.Dob,
                Phonenumber = employee.Phonenumber,
                Email = employee.Email,
                Position = employee.Position,
                Deptno = employee.Deptno,
                Employeetype = employee.Employeetype,
                Level = employee.Level,
                Lastupdateddate = employee.Lastupdateddate,
                Status = employee.Status,
                StatusReason = employee.Statusreason
            };
        }
    }
}
