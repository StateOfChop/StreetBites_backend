using StreetBites.Models;

namespace StreetBites.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetActiveAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task<bool> HasOrdersAsync(Guid productId);
        Task SaveAsync();
    }
}
