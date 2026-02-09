using Microsoft.AspNetCore.Mvc;
using StreetBites.Dtos;
using StreetBites.Models;
using StreetBites.Services;

namespace StreetBites.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La ruta será: api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var user = new User 
                { 
                    Name = dto.Name, 
                    Email = dto.Email,
                    PasswordHash = "" // Se cifra en el servicio
                };
                
                var result = await _authService.Register(user, dto.Password);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    message = ex.Message,
                    inner = ex.InnerException?.Message 
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.Login(dto.Email, dto.Password);
                return Ok(new { token }); // Le devolvemos el JWT a Angular
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
