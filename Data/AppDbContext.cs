using Microsoft.EntityFrameworkCore;
using ProductService.Models; 

namespace ProductService.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<Product> Products { get; set; } = null!;
        protected override void OnModelCreating (ModelBuilder modelBuilder){
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}