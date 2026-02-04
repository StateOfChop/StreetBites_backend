using StreetBites.Models;

namespace StreetBites.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task SaveAsync();
    }
}
