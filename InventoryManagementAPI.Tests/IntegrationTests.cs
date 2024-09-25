using FluentAssertions;
using InventoryManagementAPI.Models;
using System.Net.Http.Json;

namespace InventoryManagementAPI.Tests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public IntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsSuccessStatusCode_WithProducts()
        {
            // Arrange
            var request = "/api/Products";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            products.Should().NotBeNull();
            products.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetProductById_ReturnsSuccessStatusCode_WhenProductExists()
        {
            // Arrange
            var request = "/api/Products/5";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var product = await response.Content.ReadFromJsonAsync<Product>();
            product.Should().NotBeNull();
            product.Id.Should().Be(5);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var request = "/api/Products/9999";

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedStatusCode()
        {
            // Arrange
            var request = "/api/Products";
            var newProduct = new Product
            {
                Name = "Test Product",
                Price = 50.0m,
                Stock = 10,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var response = await _client.PostAsJsonAsync(request, newProduct);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
            createdProduct.Should().NotBeNull();
            createdProduct.Name.Should().Be("Test Product");
            createdProduct.Price.Should().Be(50.0m);
            createdProduct.Stock.Should().Be(10);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var request = "/api/Products/1";

            // Act
            var response = await _client.DeleteAsync(request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync(request);
            getResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddStock_ReturnsNoContent_WhenStockIsAdded()
        {
            // Arrange
            var productId = 3;
            var request = $"/api/Products/{productId}/stock";
            int quantityToAdd = 15;

            // Act
            var response = await _client.PostAsJsonAsync(request, quantityToAdd);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/Products/{productId}");
            getResponse.IsSuccessStatusCode.Should().BeTrue();
            var product = await getResponse.Content.ReadFromJsonAsync<Product>();
            product.Stock.Should().Be(300 + quantityToAdd);
        }

        [Fact]
        public async Task RemoveStock_ReturnsNoContent_WhenStockIsRemoved()
        {
            // Arrange
            var productId = 2;
            var request = $"/api/Products/{productId}/stock";
            int quantityToRemove = 10;

            // Act
            var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, request)
            {
                Content = JsonContent.Create(quantityToRemove)
            };
            var response = await _client.SendAsync(deleteRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _client.GetAsync($"/api/Products/{productId}");
            getResponse.IsSuccessStatusCode.Should().BeTrue();
            var product = await getResponse.Content.ReadFromJsonAsync<Product>();
            product.Stock.Should().Be(200 - quantityToRemove);
        }
    }
}