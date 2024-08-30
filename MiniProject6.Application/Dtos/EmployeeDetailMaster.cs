using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class EmployeeDetailMaster
    {
        public int? empNo { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly? Dob { get; set; }
        public string Address { get; set; } = null!;
        public int? Phonenumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public int? Deptno { get; set; }
        public string? Employeetype { get; set; }
        public int? Level { get; set; }
        public DateTime? Lastupdateddate { get; set; }
        public string? Status { get; set; }
        public string? StatusReason { get; set; }
    }
}
