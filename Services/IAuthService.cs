using StreetBites.Models;

namespace StreetBites.Services
{
    public interface IAuthService
    {
        Task<string> Register(User user, string password);
        Task<string> Login(string email, string password);
    }
}
