using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos.Project
{
    public class EmployeeProjectAssignmentDto
    {
        public int EmpNo { get; set; }
        public string FullName { get; set; }
        public int ProjNo { get; set; }
        public string ProjName { get; set; }
        public string RoleInProject { get; set; }
    }

}
