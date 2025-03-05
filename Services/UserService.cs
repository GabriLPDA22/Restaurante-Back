using CineAPI.Models.DTOs;
using CineAPI.Repositories.Interfaces;
using CineAPI.Services.Interfaces;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Obtener usuario por Google ID
        public async Task<Users?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _userRepository.GetByGoogleIdAsync(googleId);
        }

        // Agregar usuario con encriptación segura
        public async Task AddUserAsync(Users user)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email.ToLowerInvariant());
            if (existingUser != null)
            {
                throw new Exception("El Email ya está registrado.");
            }

            // ✅ Validar que la contraseña no sea null o vacía
            if (string.IsNullOrEmpty(user.Password))
            {
                throw new Exception("La contraseña no puede estar vacía.");
            }

            // ✅ Hashear la contraseña antes de guardar
            user.Password = HashPassword(user.Password);
            user.Email = user.Email.ToLowerInvariant();

            await _userRepository.AddAsync(user);
        }

        // Eliminar usuario
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            await _userRepository.DeleteAsync(userId);
        }

        // Obtener todos los usuarios
        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // Obtener usuario por ID
        public async Task<Users?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        // Obtener usuario por Email
        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email.ToLowerInvariant());
        }

        // Actualizar usuario completo
        public async Task UpdateUserAsync(Users user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.UserID);
            if (existingUser == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Si se proporciona una nueva contraseña, la hasheamos antes de actualizar
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = HashPassword(user.Password);
            }

            // Normalizamos el email
            user.Email = user.Email.ToLowerInvariant();

            await _userRepository.UpdateAsync(user);
        }

        // Actualizar información básica del usuario
        public async Task UpdateUserBasicInfo(int userId, UserUpdateDto updateDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Actualizar solo los campos básicos
            existingUser.Nombre = updateDto.Nombre;
            existingUser.Email = updateDto.Email.ToLowerInvariant();
            existingUser.Telefono = updateDto.Telefono;
            existingUser.FechaNacimiento = updateDto.FechaNacimiento;

            // Mantener los demás campos sin cambios
            await _userRepository.UpdateAsync(existingUser);
        }

        // Método para encriptar contraseñas con BCrypt
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Método para verificar contraseñas (comparación con BCrypt)
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        // Obtener o crear usuario desde Google OAuth
        public async Task<Users> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
        {
            var emailNormalized = payload.Email.ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(emailNormalized);

            if (user == null)
            {
                user = new Users
                {
                    Nombre = payload.Name,
                    Email = emailNormalized,
                    GoogleId = payload.Subject, 
                    PictureUrl = payload.Picture, 
                    Roles = new string[] { "User" }
                };

                await _userRepository.AddAsync(user);
                Console.WriteLine($"Nuevo usuario creado: {user.Nombre} - {user.Email} con imagen {user.PictureUrl}");
            }
            else
            {
                Console.WriteLine($"Usuario ya existe: {user.Nombre} - {user.Email} con imagen {user.PictureUrl}");
            }

            return user;
        }
    }
}