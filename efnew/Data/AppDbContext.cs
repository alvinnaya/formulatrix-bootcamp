using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class AppDbContext : IdentityDbContext<Users>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Postingan> Postingan { get; set; }
    public DbSet<Comment> Comment { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Users>(entity =>
        {
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasMany(u => u.Postingan)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Postingan>(entity =>
        {
            entity.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Content)
                .IsRequired();

            entity.Property(p => p.UserId)
                .IsRequired();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Postingan)
                .HasForeignKey(c => c.PostinganId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Comment>(entity =>
        {
            entity.Property(c => c.Isi)
                .IsRequired();

            entity.Property(c => c.UserId)
                .IsRequired();
        });
    }
}
