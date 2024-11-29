using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class EmployeeWithDepartmentDto
    {
        public int Empno { get; set; }
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Position { get; set; }
        public string? DeptName { get; set; }
        public string Address { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string? Sex { get; set; }
        public int? Deptno { get; set; }
        public string? Employeetype { get; set; }
        public int? Level { get; set; }
        public DateTime? Lastupdateddate { get; set; }
        public int? Nik { get; set; }
        public string? Status { get; set; }
        public string? Statusreason { get; set; }
        public int? Salary { get; set; }
        public int? SpvEmpNo { get; set; }
        public string? SpvEmpName { get; set; }
        public int? DependentNo { get; set; }
        public string? DependentName { get; set; }
    }
}
