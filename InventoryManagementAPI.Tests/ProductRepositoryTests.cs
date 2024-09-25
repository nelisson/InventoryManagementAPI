using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using InventoryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace InventoryManagementAPI.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly ProductRepository _repository;
        private readonly InventoryContext _context;
        private readonly DbContextOptions<InventoryContext> _contextOptions;
        private readonly string _databaseName;

        public ProductRepositoryTests()
        {
            _databaseName = $"Filename={Guid.NewGuid()}.db";
            _contextOptions = new DbContextOptionsBuilder<InventoryContext>()
                        .UseSqlite(_databaseName)
                        .Options;

            _context = new InventoryContext(_contextOptions);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _repository = new ProductRepository(_context);

            // Seed the database with initial data
            _context.Products.AddRange(new List<Product>
            {
                new() { Name = "Product A", Price = 10.0m, Stock = 100, UpdatedAt = DateTime.UtcNow },
                new() { Name = "Product B", Price = 20.0m, Stock = 200, UpdatedAt = DateTime.UtcNow },
                new() { Name = "Product C", Price = 30.0m, Stock = 300, UpdatedAt = DateTime.UtcNow }
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Act
            var products = await _repository.GetAllAsync();

            // Assert
            products.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var existingProduct = await _context.Products.FirstAsync();

            // Act
            var product = await _repository.GetByIdAsync(existingProduct.Id);

            // Assert
            product.Should().NotBeNull();
            product?.Name.Should().Be(existingProduct.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var product = await _repository.GetByIdAsync(999);

            // Assert
            product.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            // Arrange
            var newProduct = new Product { Name = "Product D", Price = 40.0m, Stock = 400 };

            // Act
            await _repository.AddAsync(newProduct);
            var product = await _repository.GetByIdAsync(newProduct.Id);

            // Assert
            product.Should().NotBeNull();
            product?.Name.Should().Be("Product D");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var productToUpdate = await _context.Products.FirstAsync();
            productToUpdate.Name = "Updated Product";

            // Act
            await _repository.UpdateAsync(productToUpdate);
            var updatedProduct = await _repository.GetByIdAsync(productToUpdate.Id);

            // Assert
            updatedProduct?.Name.Should().Be("Updated Product");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var productToUpdate = new Product { Id = 999, Name = "Non-Existent Product", Price = 50.0m, Stock = 500 };

            // Act
            Func<Task> act = async () => await _repository.UpdateAsync(productToUpdate);

            // Assert
            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var productToDelete = await _context.Products.FirstAsync();

            // Act
            await _repository.DeleteAsync(productToDelete.Id);
            var deletedProduct = await _repository.GetByIdAsync(productToDelete.Id);

            // Assert
            deletedProduct.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _repository.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync<Exception>();
            var productCount = await _context.Products.CountAsync();
            productCount.Should().Be(3);
        }

        [Fact]
        public async Task AddStockAsync_ShouldIncreaseStock_WhenProductExists()
        {
            // Arrange
            var product = await _context.Products.FirstAsync();
            var initialStock = product.Stock;
            var quantityToAdd = 50;

            // Act
            await _repository.AddStockAsync(product.Id, quantityToAdd);
            var updatedProduct = await _repository.GetByIdAsync(product.Id);

            // Assert
            updatedProduct?.Stock.Should().Be(initialStock + quantityToAdd);
        }

        [Fact]
        public async Task AddStockAsync_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _repository.AddStockAsync(999, 50);

            // Assert
            await act.Should().NotThrowAsync<Exception>();
            var productCount = await _context.Products.CountAsync();
            productCount.Should().Be(3);
        }

        [Fact]
        public async Task RemoveStockAsync_ShouldDecreaseStock_WhenProductExists()
        {
            // Arrange
            var product = await _context.Products.FirstAsync();
            var initialStock = product.Stock;
            var quantityToRemove = 30;

            // Act
            await _repository.RemoveStockAsync(product.Id, quantityToRemove);
            var updatedProduct = await _repository.GetByIdAsync(product.Id);

            // Assert
            updatedProduct?.Stock.Should().Be(initialStock - quantityToRemove);
        }

        [Fact]
        public async Task RemoveStockAsync_ShouldDoNothing_WhenProductDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _repository.RemoveStockAsync(999, 30);

            // Assert
            await act.Should().NotThrowAsync<Exception>();
            var productCount = await _context.Products.CountAsync();
            productCount.Should().Be(3);
        }

        [Fact]
        public async Task RemoveStockAsync_ShouldNotReduceStockBelowZero()
        {
            // Arrange
            var product = await _context.Products.FirstAsync();
            var quantityToRemove = product.Stock + 10; // More than available stock

            // Act
            await _repository.RemoveStockAsync(product.Id, quantityToRemove);
            var updatedProduct = await _repository.GetByIdAsync(product.Id);

            // Assert
            updatedProduct?.Stock.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task UpdatedAt_ShouldBeUpdated_WhenProductIsUpdated()
        {
            // Arrange
            var product = await _context.Products.FirstAsync();
            var initialUpdatedAt = product.UpdatedAt;

            await Task.Delay(1000);

            // Act
            product.Name = "Updated Product Name";
            await _repository.UpdateAsync(product);

            // Assert
            product.UpdatedAt.Should().BeAfter(initialUpdatedAt); 
        }

        [Fact]
        public async Task Should_ThrowConcurrencyException_WhenProductIsUpdatedConcurrently()
        {
            // Arrange
            var product = await _context.Products.FirstAsync();

            var externalContext = new InventoryContext(_contextOptions);
            var externalProduct = await externalContext.Products.FindAsync(product.Id);

            await Task.Delay(1000);

            product.Name = "Updated by First Context";
            await _repository.UpdateAsync(product); 

            // Act
            externalProduct.Name = "Updated by Second Context";

            // Assert
            Func<Task> act = async () => {
                externalContext.Products.Update(externalProduct);
                await externalContext.SaveChangesAsync();
            };

            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }



        public void Dispose()
        {
            _context.Database.CloseConnection(); 
            _context.Dispose();  
            if (System.IO.File.Exists(_databaseName))
            {
                System.IO.File.Delete(_databaseName);  
            }
        }
    }
}
