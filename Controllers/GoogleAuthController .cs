using Microsoft.AspNetCore.Mvc;
using CineAPI.Services.Interfaces;
using Google.Apis.Auth;
using System.Threading.Tasks;

namespace Restaurante.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public GoogleAuthController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Login con Google
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "812430762080-ktkqmivkpjo15cnid2ch4dd217r04v4l.apps.googleusercontent.com" }
                });

                var user = await _userService.GetOrCreateUserAsync(payload);

                return Ok(new
                {
                    user.UserID,
                    user.Nombre,
                    user.Email,
                    user.PictureUrl,
                    user.Roles
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class GoogleLoginRequest
    {
        public string Token { get; set; }
    }
}
