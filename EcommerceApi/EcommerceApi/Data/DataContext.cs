using EcommerceApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EcommerceApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSize> ProductSize { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasOne(s => s.Owner)
                .WithMany()
                .HasForeignKey(s => s.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
                .HasMany(s => s.Managers)
                .WithMany(u => u.Stores)
                .UsingEntity<Dictionary<string, object>>(
                    "StoreManager",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Store>().WithMany().HasForeignKey("StoreId"),
                    j =>
                    {
                        j.HasKey("UserId", "StoreId");
                    });
        }

    }
}
