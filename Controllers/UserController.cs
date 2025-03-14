using CineAPI.Models.DTOs;
using CineAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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
            catch (Exception ex)
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
                await _userService.AddUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdate)
        {
            try
            {
                Console.WriteLine($"Actualizando usuario ID {id}: Nombre={userUpdate.Nombre}, Email={userUpdate.Email}, Telefono={userUpdate.Telefono}, FechaNacimiento={userUpdate.FechaNacimiento}");

                await _userService.UpdateUserBasicInfo(id, userUpdate);
                
                // Obtener el usuario actualizado para devolverlo
                var updatedUser = await _userService.GetUserByIdAsync(id);
                return Ok(updatedUser);
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("update-by-email")]
        public async Task<IActionResult> UpdateUserByEmail([FromBody] UserUpdateDto updateDto)
        {
            try
            {
                Console.WriteLine($"Actualizando usuario por email: {updateDto.Email}, Nombre={updateDto.Nombre}, Telefono={updateDto.Telefono}, FechaNacimiento={updateDto.FechaNacimiento}");

                // 1. Buscar el usuario por email
                var existingUser = await _userService.GetUserByEmailAsync(updateDto.Email);
                if (existingUser == null)
                    return NotFound(new { Message = "Usuario no encontrado" });

                Console.WriteLine($"Usuario encontrado: ID={existingUser.UserID}, Nombre={existingUser.Nombre}");

                // 2. Crear un UserUpdateDto completo
                var userUpdateDto = new UserUpdateDto
                {
                    Nombre = updateDto.Nombre,
                    Email = updateDto.Email,
                    Telefono = updateDto.Telefono,
                    FechaNacimiento = updateDto.FechaNacimiento
                };

                // 3. Usar el mismo método de actualización básica
                await _userService.UpdateUserBasicInfo(existingUser.UserID, userUpdateDto);
                
                // 4. Obtener el usuario actualizado
                var updatedUser = await _userService.GetUserByIdAsync(existingUser.UserID);
                Console.WriteLine("Usuario actualizado correctamente");
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar por email: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}