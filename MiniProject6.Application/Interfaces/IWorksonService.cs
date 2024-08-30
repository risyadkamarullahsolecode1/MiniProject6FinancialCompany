﻿using MiniProject6.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Application.Interfaces
{
    public interface IWorksonService
    {
        Task<Workson> AssignEmployeeWorkson(int empno,Workson workson);
        Task<Workson> UpdateEmployeeWorkson(int empno, Workson workson);
    }
}
