using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

    public class AppDbContext : DbContext
    {

        public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }

       

        
    }

