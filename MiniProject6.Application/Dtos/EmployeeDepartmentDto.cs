using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class EmployeeDepartmentDto
    {
        public int EmpNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public int? Deptno { get; set; }
        public string? Position { get; set; }
    }
}
