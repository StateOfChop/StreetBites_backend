using Microsoft.EntityFrameworkCore;
namespace StreetBites.Models
{
    public enum OrderStatus
    {
        PENDING,
        PREPARING,
        DELIVERED,
        CANCELLED
    }
}
