using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreetBites.Data;
using StreetBites.Dtos;
using StreetBites.Interfaces;
using StreetBites.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace StreetBites.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly AppDbContext _context;

        public OrdersController(IOrderRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// GET /api/orders - Returns all orders for ADMIN, or user-specific orders for regular users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "ADMIN")
            {
                return Ok(await _repository.GetAllAsync());
            }

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            return Ok(await _repository.GetByUserIdAsync(parsedUserId));
        }

        /// <summary>
        /// GET /api/orders/{id} - Get a specific order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Only allow ADMIN or the order owner to view the order
            if (userRole != "ADMIN" && order.UserId.ToString() != userId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        /// <summary>
        /// POST /api/orders - Create a new order
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { message = "Order must contain at least one item" });
            }

            // Validate all products exist and are active
            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            // Check if all products exist
            var missingProducts = productIds.Except(products.Select(p => p.Id)).ToList();
            if (missingProducts.Any())
            {
                return BadRequest(new { message = $"Products not found: {string.Join(", ", missingProducts)}" });
            }

            // Check if all products are active
            var inactiveProducts = products.Where(p => !p.IsActive).Select(p => p.Name).ToList();
            if (inactiveProducts.Any())
            {
                return BadRequest(new { message = $"Products not available: {string.Join(", ", inactiveProducts)}" });
            }

            // Validate stock availability for all items
            foreach (var itemDto in dto.Items)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);
                if (product.Stock < itemDto.Quantity)
                {
                    return BadRequest(new { message = $"Insufficient stock for '{product.Name}'. Available: {product.Stock}, Requested: {itemDto.Quantity}" });
                }
            }

            // Create the order
            var order = new Order
            {
                UserId = parsedUserId,
                Status = OrderStatus.PENDING,
                CreatedAt = DateTime.UtcNow
            };

            // Create order items with price history and deduct stock
            decimal total = 0;
            foreach (var itemDto in dto.Items)
            {
                var product = products.First(p => p.Id == itemDto.ProductId);
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                };
                order.Items.Add(orderItem);
                total += product.Price * itemDto.Quantity;

                // Deduct stock
                product.Stock -= itemDto.Quantity;
            }

            order.Total = total;

            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            // Reload the order with navigation properties for response
            var createdOrder = await _repository.GetByIdAsync(order.Id);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, createdOrder);
        }

        /// <summary>
        /// PUT /api/orders/{id}/status - Update order status (ADMIN only)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            order.Status = dto.Status;
            await _repository.UpdateAsync(order);
            await _repository.SaveAsync();

            return Ok(new { message = "Order status updated successfully", order });
        }

        /// <summary>
        /// PUT /api/orders/{id}/cancel - Cancel an order (owner only, must be PENDING)
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Check if user owns this order
            if (order.UserId != parsedUserId)
            {
                return Forbid();
            }

            // Only allow cancellation of PENDING orders
            if (order.Status != OrderStatus.PENDING)
            {
                return BadRequest(new { message = "Only PENDING orders can be cancelled" });
            }

            // Restore stock for each item in the cancelled order
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                }
            }

            order.Status = OrderStatus.CANCELLED;
            await _repository.UpdateAsync(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order cancelled successfully", order });
        }
    }
}

