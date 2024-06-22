using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductsManageApp.Models;
using ProductsManageApp.Models.Configuration;

namespace ProductsManageApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Product>()
            //    .HasOne(p => p.Users)
            //    .WithMany(u => u.Products)
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
