using Microsoft.EntityFrameworkCore;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Infrastructure.Data.Repository
{
    public class WorksonRepository : IWorksonRepository
    {
        private readonly CompanyContext _context;

        public WorksonRepository(CompanyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Workson>> GetAllWorkson()
        {
            return await _context.Worksons.ToListAsync();
        }
        public async Task<Workson> GetWorksonById(int empNo, int projNo)
        {
            return await _context.Worksons.FindAsync(empNo, projNo);
        }
        public async Task<Workson> AddWorkson(Workson workson)
        {
            _context.Worksons.Add(workson);
            await _context.SaveChangesAsync();
            return workson;
        }
        public async Task<Workson> UpdateWorkson(Workson workson)
        {
            _context.Worksons.Update(workson);
            await _context.SaveChangesAsync();
            return workson;

        }
        public async Task<bool> DeleteWorkson(int empNo, int projNo)
        {
            var deleted = await _context.Worksons.FindAsync(empNo, projNo);
            if (deleted == null)
            {
                return false;
            }
            _context.Worksons.Remove(deleted);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Workson>> GetWorksonByEmpNo(int empNo)
        {
            return await _context.Worksons.Where(d => d.Empno == empNo).ToListAsync();
        }

        public async Task<IEnumerable<object>> GetEmployeesByProjectAsync(int projNo)
        {
            var employeeProjectDetails = await _context.Worksons
                .Where(w => w.Projno == projNo)
                .Include(w => w.EmpnoNavigation)  // Include Employee details
                .Include(w => w.ProjnoNavigation) // Include Project details
                .Select(w => new
                {
                    EmployeeNo = w.EmpnoNavigation.Empno,
                    EmployeeName = $"{w.EmpnoNavigation.Fname} {w.EmpnoNavigation.Lname}",
                    ProjectNo = w.Projno,
                    ProjectName = w.ProjnoNavigation.Projname,
                    TotalHours = w.Hoursworked,
                    DateWorked = w.Dateworked
                })
                .ToListAsync();

            return employeeProjectDetails;
        }
    }
}
