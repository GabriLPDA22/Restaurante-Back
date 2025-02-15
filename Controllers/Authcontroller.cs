using CineAPI.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // 🔥 Nuevo método: Recibe el token de Google, lo verifica y autentica al usuario
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            Console.WriteLine($"Received Token: {request.Token}"); // 👀 Imprimir token recibido
            var payload = await VerifyGoogleToken(request.Token);

            if (payload == null)
            {
                Console.WriteLine("Google token verification failed!"); // 👀 Verificar errores en consola del backend
                return BadRequest(new { Message = "Invalid Google token." });
            }

            var user = await _userService.GetOrCreateUserAsync(payload);

            return Ok(new
            {
                user.UserID,
                user.Nombre,
                user.Correo,
                user.PictureUrl,
                user.Roles
            });
        }


        // ✅ Método para obtener información del usuario autenticado
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var googleId = User.FindFirst("sub")?.Value; // Obtener Google ID
            if (string.IsNullOrEmpty(googleId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var user = await _userService.GetUserByGoogleIdAsync(googleId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new
            {
                user.UserID,
                user.Nombre,
                user.Correo,
                user.PictureUrl,
                user.Roles
            });
        }

        // ✅ Método para verificar el token de Google
        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "812430762080-ktkqmivkpjo15cnid2ch4dd217r04v4l.apps.googleusercontent.com" } // ⚠️ Reemplaza con tu Client ID
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
    }

    public class GoogleLoginRequest
    {
        public string Token { get; set; }
    }
}
