using Microsoft.AspNetCore.Mvc;
using CineAPI.Services.Interfaces;
using System.Threading.Tasks;
using BCrypt.Net; // Asegúrate de tener este using

namespace Restaurante.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Registro normal (email y contraseña)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Validación de campos obligatorios
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "El email y la contraseña son obligatorios." });
            }
            
            // Opcional: Normalizar el email (por ejemplo, a minúsculas)
            var normalizedEmail = request.Email.ToLowerInvariant();
            
            var user = new Users
            {
                Nombre = string.IsNullOrEmpty(request.Nombre) ? "Sin Nombre" : request.Nombre,
                Email = normalizedEmail,
                Password = _userService.HashPassword(request.Password)
            };

            try
            {
                await _userService.AddUserAsync(user);
                return Ok(new { Message = "Usuario registrado correctamente." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // Login normal
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Normalizar el email para la búsqueda
            var normalizedEmail = request.Email.ToLowerInvariant();
            
            var user = await _userService.GetUserByEmailAsync(request.Email);
            Console.WriteLine(user == null
                ? "Usuario no encontrado"
                : $"Usuario encontrado: {user.Email}, Hash: {user.Password}");
            if (user != null)
            {
                bool verify = _userService.VerifyPassword(request.Password, user.Password);
                Console.WriteLine($"Verificación: {verify}");
            }


            return Ok(new
            {
                user.UserID,
                user.Nombre,
                user.Email,
                user.PictureUrl,
                user.Roles
            });
        }
    }

    public class RegisterRequest
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
