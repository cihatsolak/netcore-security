using DataProtection.Web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProtection.Web.Models.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
