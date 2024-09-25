using InventoryManagementAPI.Models;
using InventoryManagementAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await repository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            await repository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await repository.UpdateAsync(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await repository.GetByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/stock")]
        public async Task<IActionResult> AddStock(int id, [FromBody] int quantity)
        {
            await repository.AddStockAsync(id, quantity);
            return NoContent();
        }

        [HttpDelete("{id}/stock")]
        public async Task<IActionResult> RemoveStock(int id, [FromBody] int quantity)
        {
            await repository.RemoveStockAsync(id, quantity);
            return NoContent();
        }
    }
}
