using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Exceptions;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        // GET /api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var list = await _db.Products.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        // GET /api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            //Gestion centralisée des exceptions
            if (product == null) throw new NotFoundException($"Product with id {id} not found");
            //if (product == null) return NotFound();
            return Ok(product);
        }

        // POST /api/products
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // PUT /api/products/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product update)
        {
            if (id != update.Id) return BadRequest("Id mismatch.");
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            //Gestion centralisée des exceptions
            if (product == null) throw new NotFoundException($"Product with id {id} not found");

            //if (product == null) return NotFound();

            // Mise à jour simple
            product.Name = update.Name;
            product.Description = update.Description;
            product.Price = update.Price;
            product.Stock = update.Stock;
            product.SupplierId = update.SupplierId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            //Gestion centralisée des exception
            if (product == null) throw new NotFoundException($"Product with id {id} not found");
            //if (product == null) return NotFound();

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Optionnel : endpoint atomique pour réserver/décrémenter le stock (utile pour orders)
        [HttpPost("{id:int}/{qty:int}/reserve")]
        public async Task<IActionResult> Reserve(int id, int qty)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
            // Gestion centralisée des exceptions 
            if (product == null) throw new NotFoundException($"Product with id {id} not found");

            //if (product == null) return NotFound();
            if (product.Stock < qty) return BadRequest("Insufficient stock.");

            product.Stock -= qty;
            await _db.SaveChangesAsync();
            return Ok(product);
        }
    }
}
