using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Dtos
{
    public class SearchDto
    {
        public int? EmpNo { get; set; }
        public string? EmployeeName { get; set; } = null;
        public string? DepartmentName { get; set; } = null;
        public string? Position { get; set; } = null;
        public int? Level { get; set; } = null;
        public string? EmploymentType { get; set; } = null;
        public DateTime? LastUpdatedDate { get; set; }
        public string? Keyword { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public string? SortOrder { get; set; } = "asc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
