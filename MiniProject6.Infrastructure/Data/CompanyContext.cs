using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniProject6.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject6.Infrastructure.Data
{
    public partial class CompanyContext: IdentityDbContext<AppUser>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Workson> Worksons { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<Location> Locations { get; set; }

        public CompanyContext(DbContextOptions<CompanyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Deptno).HasName("departments_pkey");

                entity.HasOne(d => d.LocationNavigation).WithMany(p => p.Departments).HasConstraintName("fk_location");

                entity.HasOne(d => d.MgrempnoNavigation).WithMany(p => p.DepartmentMgrempnoNavigations)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("departments_mgrempno_fkey");

                entity.HasOne(d => d.SpvempnoNavigation).WithMany(p => p.DepartmentSpvempnoNavigations).HasConstraintName("fk_spvempno");
            });

            modelBuilder.Entity<Dependent>(entity =>
            {
                entity.HasKey(e => e.Dependentno).HasName("dependents_pkey");

                entity.HasOne(d => d.EmpnoNavigation).WithMany(p => p.Dependents)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("dependents_empno_fkey");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Empno).HasName("employees_pkey");

                entity.Property(e => e.Lastupdateddate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.DeptnoNavigation).WithMany(p => p.Employees).HasConstraintName("fk_deptno");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Locations).HasName("location_pkey");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Projno).HasName("projects_pkey");

                entity.HasOne(d => d.DeptnoNavigation).WithMany(p => p.Projects)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("projects_deptno_fkey");
            });

            modelBuilder.Entity<Workson>(entity =>
            {
                entity.HasKey(e => new { e.Empno, e.Projno }).HasName("workson_pkey");

                entity.HasOne(d => d.EmpnoNavigation).WithMany(p => p.Worksons).HasConstraintName("workson_empno_fkey");

                entity.HasOne(d => d.ProjnoNavigation).WithMany(p => p.Worksons).HasConstraintName("workson_projno_fkey");
            });

            modelBuilder.Entity<Employee>()
            .HasOne(e => e.AppUser)
            .WithMany(a => a.Employees)
            .HasForeignKey(e => e.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
