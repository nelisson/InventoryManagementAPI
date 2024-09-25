using InventoryManagementAPI.Models;

namespace InventoryManagementAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task AddStockAsync(int id, int quantity);
        Task RemoveStockAsync(int id, int quantity);
    }
}
