using Microsoft.EntityFrameworkCore;
using StreetBites.Data;
using StreetBites.Interfaces;
using StreetBites.Models;

namespace StreetBites.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Order>> GetAllAsync() =>
            await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .ToListAsync();

        public async Task<Order?> GetByIdAsync(Guid id) =>
            await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId) =>
            await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

        public async Task AddAsync(Order order) => await _context.Orders.AddAsync(order);

        public Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}

