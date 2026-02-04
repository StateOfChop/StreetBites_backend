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
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET /api/products - Returns active products for users, all products for ADMIN (with query param)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] bool includeInactive = false)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only ADMIN can see inactive products
            if (includeInactive && userRole == "ADMIN")
            {
                return Ok(await _repository.GetAllAsync());
            }

            return Ok(await _repository.GetActiveAsync());
        }

        /// <summary>
        /// GET /api/products/{id} - Get a specific product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Non-admin users cannot see inactive products
            if (!product.IsActive && userRole != "ADMIN")
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// POST /api/products - Create a new product (ADMIN only)
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
        /// PUT /api/products/{id} - Update a product (ADMIN only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

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
        /// DELETE /api/products/{id} - Deactivate a product (ADMIN only)
        /// Business rule: Products with orders cannot be deleted, only deactivated
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Check if product has orders - if so, soft delete only
            var hasOrders = await _repository.HasOrdersAsync(id);

            if (hasOrders)
            {
                // Soft delete - just deactivate the product
                product.IsActive = false;
                await _repository.UpdateAsync(product);
                await _repository.SaveAsync();

                return Ok(new { message = "Product deactivated (has existing orders)", product });
            }

            // If no orders, we could hard delete, but soft delete is safer
            // Following the business rule: "No eliminar productos con pedidos"
            // We'll still soft delete for consistency
            product.IsActive = false;
            await _repository.UpdateAsync(product);
            await _repository.SaveAsync();

            return Ok(new { message = "Product deactivated successfully", product });
        }
    }
}
