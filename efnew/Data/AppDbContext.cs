using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

    public class AppDbContext : DbContext
    {

         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     // Example: Configuring a SQLite database connection
    //     optionsBuilder.UseSqlite(@"Data Source=MyDatabase.db");
    // }

       

        public DbSet<Product> Products { get; set; }
        public DbSet<Users> Users {get; set;}
        public DbSet<Postingan> Postingan { get; set; }
    }

