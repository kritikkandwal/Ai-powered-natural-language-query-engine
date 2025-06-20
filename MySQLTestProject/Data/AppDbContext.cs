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

        // 👇 Add this method *inside* AppDbContext class
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("employee"); // <-- exact match with MySQL
        }
    }

    public class Employee
    {
        [Key]
        public int EmpID { get; set; }
        public decimal Salary { get; set; }
    }
}
