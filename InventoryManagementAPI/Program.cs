using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;
using InventoryManagementAPI.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryContext>();

    if (!dbContext.Products.Any())
    {
        dbContext.Products.AddRange(new List<Product>
        {
            new() { Name = "Product A", Price = 10.0m, Stock = 100, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Product B", Price = 20.0m, Stock = 200, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Product C", Price = 30.0m, Stock = 300, UpdatedAt = DateTime.UtcNow }
        });
        dbContext.SaveChanges();
    }
}


app.Run();
