using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class EmployeeSortedDto
    {
        public int? EmpNo { get; set; }
        public string Fname { get; set; } = null!;
        public string? Position { get; set; }
        public string? Deptname { get; set; }
        public string? Employeetype { get; set; }
        public int? Level { get; set; }
        public DateTime? Lastupdateddate { get; set; }
    }
}
