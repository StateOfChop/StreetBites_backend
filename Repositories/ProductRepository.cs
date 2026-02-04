using Microsoft.EntityFrameworkCore;
using StreetBites.Data;
using StreetBites.Interfaces;
using StreetBites.Models;

namespace StreetBites.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.ToListAsync();

        public async Task<IEnumerable<Product>> GetActiveAsync() =>
            await _context.Products.Where(p => p.IsActive).ToListAsync();

        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product) =>
            await _context.Products.AddAsync(product);

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public async Task<bool> HasOrdersAsync(Guid productId) =>
            await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);

        public async Task SaveAsync() =>
            await _context.SaveChangesAsync();
    }
}
