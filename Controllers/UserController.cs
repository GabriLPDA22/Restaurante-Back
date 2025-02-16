using CineAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { Message = "Usuario no encontrado" });
            
            return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Users user)
        {
            try
            {
                await _userService.AddUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users user)
        {
            if (id != user.UserID)
                return BadRequest(new { Message = "El ID proporcionado no coincide con el usuario." });
            
            try
            {
                await _userService.UpdateUserAsync(user);
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        
        // Endpoint para obtener el usuario actual (ej. a través de Google OAuth)
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }
            
            // DEBUG: Mostrar los claims recibidos
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            
            var googleId = User.FindFirst("sub")?.Value; // Google ID del token
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
                user.Email,
                user.PictureUrl,
                user.Roles
            });
        }
    }
}
