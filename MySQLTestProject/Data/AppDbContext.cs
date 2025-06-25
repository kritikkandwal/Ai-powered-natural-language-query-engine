using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MySQLTestProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
                entity.Property(e => e.EmpID).HasColumnName("EmpID");
                entity.Property(e => e.Salary).HasColumnName("Salary");
            });
        }
    }

    public class Employee
    {
        [Key]
        public int EmpID { get; set; }
        public decimal Salary { get; set; }
    }
}