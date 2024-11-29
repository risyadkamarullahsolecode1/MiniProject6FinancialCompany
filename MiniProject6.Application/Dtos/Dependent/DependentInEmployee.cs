using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos.Dependent
{
    public class DependentInEmployee
    {
        public int DependentId { get; set; }
        public string? DependentName { get; set; }
        public string? Sex { get; set; }
        public DateOnly? Dob {  get; set; }
        public string? Relation {  get; set; }
    }
}
