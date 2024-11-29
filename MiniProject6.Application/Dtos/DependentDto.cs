using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class DependentDto
    {
        public int Dependentno { get; set; }
        public string Name { get; set; } = null!;
        public string? Sex { get; set; }
        public DateOnly Dob { get; set; }
        public string Relationship { get; set; } = null!;
        public int EmpNo { get; set; }
    }
}
