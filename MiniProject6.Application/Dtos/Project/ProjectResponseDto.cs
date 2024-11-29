using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos.Project
{
    public class ProjectResponseDto
    {
        public int ProjNo { get; set; }
        public string ProjName { get; set; }
        public int? Location { get; set; }
        public int? Deptno { get; set; }
    }

}
