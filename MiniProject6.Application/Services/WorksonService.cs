using MiniProject6.Application.Interfaces;
using MiniProject6.Domain.Entities;
using MiniProject6.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Services
{
    public class WorksonService:IWorksonService
    {
        private readonly IWorksonRepository _worksonRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public WorksonService(IWorksonRepository worksonRepository, IEmployeeRepository employeeRepository)
        {
            _worksonRepository = worksonRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Workson> AssignEmployeeWorkson(int empno, Workson workson)
        {
            var employee = await _employeeRepository.GetEmployeeById(empno);
            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found");
            }

            var workons = await _worksonRepository.UpdateWorkson(workson);
            return workons;
        }

        public Task<Workson> UpdateEmployeeWorkson(int empno, Workson workson)
        {
            throw new NotImplementedException();
        }
    }
}
