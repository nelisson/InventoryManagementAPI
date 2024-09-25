using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementAPI.Controllers;
using InventoryManagementAPI.Repositories;
using InventoryManagementAPI.Models;

namespace InventoryManagementAPI.Tests
{

    public class ProductsControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _controller = new ProductsController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", Price = 10.0m, Stock = 100 },
                new Product { Id = 2, Name = "Product B", Price = 20.0m, Stock = 200 }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<Product>>()
                .Which.Should().HaveCount(2)
                .And.BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product A", Price = 10.0m, Stock = 100 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeOfType<Product>().Which.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "New Product", Price = 30.0m, Stock = 300 };
            _mockRepository.Setup(repo => repo.AddAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(product);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdAtActionResult.RouteValues["id"].Should().Be(product.Id);
            createdAtActionResult.Value.Should().BeOfType<Product>().Which.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task Update_ReturnsNoContentResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Product", Price = 40.0m, Stock = 400 };
            _mockRepository.Setup(repo => repo.UpdateAsync(product)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenProductIdMismatch()
        {
            // Arrange
            var product = new Product { Id = 2, Name = "Updated Product", Price = 40.0m, Stock = 400 };

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Product", Price = 40.0m, Stock = 400 };
            _mockRepository.Setup(repo => repo.UpdateAsync(product)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Update_ThrowsException_WhenAnErrorOccurs()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Product", Price = 40.0m, Stock = 400 };
            _mockRepository.Setup(repo => repo.UpdateAsync(product)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Product { Name = "product" });

            // Act
            Func<Task> act = async () => await _controller.Update(1, product);

            // Assert
            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult_WhenDeletionIsSuccessful()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AddStock_ReturnsNoContentResult_WhenStockIsAdded()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.AddStockAsync(1, 10)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddStock(1, 10);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveStock_ReturnsNoContentResult_WhenStockIsRemoved()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.RemoveStockAsync(1, 5)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveStock(1, 5);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }

}