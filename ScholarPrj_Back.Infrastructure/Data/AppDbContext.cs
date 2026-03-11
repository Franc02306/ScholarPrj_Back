using Microsoft.EntityFrameworkCore;
using ScholarPrj_Back.Domain.Entities;

namespace ScholarPrj_Back.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}