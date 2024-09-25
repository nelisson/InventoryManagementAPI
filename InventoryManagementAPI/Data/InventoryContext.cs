using InventoryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Data
{
    public class InventoryContext(DbContextOptions<InventoryContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method
            base.OnModelCreating(modelBuilder);

            // Configure the RowVersion property
            modelBuilder.Entity<Product>()
                .Property(p => p.UpdatedAt)
                .IsConcurrencyToken();
        }
    }
}
