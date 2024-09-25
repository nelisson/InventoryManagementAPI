using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace InventoryManagementAPI.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext registrado
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<InventoryContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona um DbContext de teste em memória
                services.AddDbContext<InventoryContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // Cria o escopo para o serviço
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<InventoryContext>();

                    // Garante que o banco de dados esteja criado
                    db.Database.EnsureCreated();

                    // Opcional: Seed de dados de teste
                    db.Products.AddRange(new List<Product>
                {
                    new() { Name = "Product A", Price = 10.0m, Stock = 100, UpdatedAt = DateTime.UtcNow },
                    new() { Name = "Product B", Price = 20.0m, Stock = 200, UpdatedAt = DateTime.UtcNow },
                    new() { Name = "Product C", Price = 30.0m, Stock = 300, UpdatedAt = DateTime.UtcNow },
                    new() { Name = "Product D", Price = 40.0m, Stock = 400, UpdatedAt = DateTime.UtcNow },
                    new() { Name = "Product E", Price = 50.0m, Stock = 500, UpdatedAt = DateTime.UtcNow }
                });
                    db.SaveChanges();
                }
            });
        }
    }

}
