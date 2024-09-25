using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Repositories
{
    public class ProductRepository(InventoryContext context) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await context.Products.FindAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            context.Entry(product).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddStockAsync(int id, int quantity)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                product.Stock += quantity;
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveStockAsync(int id, int quantity)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                product.Stock -= quantity;
                await context.SaveChangesAsync();
            }
        }
    }
}
