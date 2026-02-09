using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetBites.Dtos;
using StreetBites.Interfaces;
using StreetBites.Models;
using System.Security.Claims;

namespace StreetBites.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /api/products - Public: active products, ADMIN can include inactive
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] bool includeInactive = false)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (includeInactive && userRole == "ADMIN")
            {
                return Ok(await _repository.GetAllAsync());
            }

            return Ok(await _repository.GetActiveAsync());
        }

        /// <summary>
        /// GET /api/products/{id} - Public
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!product.IsActive && userRole != "ADMIN")
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        /// <summary>
        /// POST /api/products - ADMIN only
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Product>> CreateProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(product);
            await _repository.SaveAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// PUT /api/products/{id} - ADMIN only
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.IsActive = dto.IsActive;

            await _repository.UpdateAsync(product);
            await _repository.SaveAsync();

            return Ok(new { message = "Product updated successfully", product });
        }

        /// <summary>
        /// DELETE /api/products/{id} - ADMIN only (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            product.IsActive = false;
            await _repository.UpdateAsync(product);
            await _repository.SaveAsync();

            return Ok(new { message = "Product deactivated successfully", product });
        }
    }
}
