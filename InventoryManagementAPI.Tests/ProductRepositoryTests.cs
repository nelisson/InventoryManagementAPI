using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using InventoryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Tests
{
    public class ProductRepositoryTests
    {
        private readonly ProductRepository _repository;
        private readonly InventoryContext _context;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new InventoryContext(options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 10, Stock = 100 };

            // Act
            await _repository.AddAsync(product);

            // Assert
            var retrievedProduct = await _repository.GetByIdAsync(product.Id);
            Assert.NotNull(retrievedProduct);
            Assert.Equal("Test Product", retrievedProduct.Name);
        }
    }
}