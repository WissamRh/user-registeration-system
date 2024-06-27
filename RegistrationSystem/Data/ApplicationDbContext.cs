using Microsoft.EntityFrameworkCore;
using RegistrationSystem.Models; // Ensure this namespace matches the location of your models

namespace RegistrationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
