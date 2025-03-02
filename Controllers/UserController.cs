using CineAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Obtén el email del usuario del token
                var email = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(email))
                {
                    // Si no hay un email en el token, intenta obtener un ID de usuario
                    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                    {
                        return Unauthorized(new { Message = "Usuario no autenticado" });
                    }

                    var userById = await _userService.GetUserByIdAsync(userId);
                    if (userById == null)
                    {
                        return NotFound(new { Message = "Usuario no encontrado" });
                    }

                    return Ok(userById);
                }

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { Message = "Usuario no encontrado" });
                }

                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
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
                // user.Telefono y user.FechaNacimiento vendrán en el JSON si se envían
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
                Console.WriteLine($"Actualizando usuario ID {id}: Nombre={user.Nombre}, Email={user.Email}, Telefono={user.Telefono}, FechaNacimiento={user.FechaNacimiento}");

                // user.Telefono y user.FechaNacimiento se mapearán desde el JSON
                await _userService.UpdateUserAsync(user);
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
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


        [HttpPut("update-by-email")]
        public async Task<IActionResult> UpdateUserByEmail([FromBody] UpdateUserDto updateDto)
        {
            try
            {
                Console.WriteLine($"Actualizando usuario por email: {updateDto.Email}, Nombre={updateDto.Nombre}, Telefono={updateDto.Telefono}, FechaNacimiento={updateDto.FechaNacimiento}");

                // 1. Buscar el usuario por email
                var existingUser = await _userService.GetUserByEmailAsync(updateDto.Email);
                if (existingUser == null)
                    return NotFound(new { Message = "Usuario no encontrado" });

                Console.WriteLine($"Usuario encontrado: ID={existingUser.UserID}, Nombre={existingUser.Nombre}");

                // 2. Actualizar solo los campos proporcionados
                if (!string.IsNullOrEmpty(updateDto.Nombre))
                    existingUser.Nombre = updateDto.Nombre;

                if (!string.IsNullOrEmpty(updateDto.Telefono))
                    existingUser.Telefono = updateDto.Telefono;

                if (!string.IsNullOrEmpty(updateDto.FechaNacimiento))
                {
                    if (DateTime.TryParse(updateDto.FechaNacimiento, out DateTime fechaNac))
                    {
                        existingUser.FechaNacimiento = fechaNac;
                        Console.WriteLine($"Fecha convertida: {fechaNac}");
                    }
                    else
                    {
                        Console.WriteLine($"No se pudo convertir la fecha: {updateDto.FechaNacimiento}");
                    }
                }

                // 3. Guardar cambios
                await _userService.UpdateUserAsync(existingUser);
                Console.WriteLine("Usuario actualizado correctamente");
                return Ok(existingUser);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al actualizar por email: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}

public class UpdateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? FechaNacimiento { get; set; }
}