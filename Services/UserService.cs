using CineAPI.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;
using CineAPI.Services.Interfaces;
using Google.Apis.Auth;

namespace CineAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Users?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _userRepository.GetByGoogleIdAsync(googleId);
        }
        public async Task AddUserAsync(Users user)
        {
            // Validar si el correo ya existe
            var existingUser = await _userRepository.GetByEmailAsync(user.Correo);
            if (existingUser != null)
            {
                throw new System.Exception("El correo ya está registrado.");
            }

            // Hashear la contraseña antes de agregar
            user.Password = HashPassword(user.Password);

            // Agregar el usuario al repositorio
            await _userRepository.AddAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            // Verificar si el usuario existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new System.Exception("Usuario no encontrado.");
            }

            // Eliminar el usuario
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<Users?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task UpdateUserAsync(Users user)
        {
            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetByIdAsync(user.UserID);
            if (existingUser == null)
            {
                throw new System.Exception("Usuario no encontrado.");
            }

            // Hashear la contraseña si se proporciona una nueva
            if (!string.IsNullOrEmpty(user.Password))
            {
                user.Password = HashPassword(user.Password);
            }

            // Actualizar el usuario
            await _userRepository.UpdateAsync(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Implementación de GetOrCreateUserAsync para Google
        public async Task<Users> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _userRepository.GetByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new Users
                {
                    Nombre = payload.Name,
                    Correo = payload.Email,
                    GoogleId = payload.Subject, // ID único de Google
                    PictureUrl = payload.Picture,
                    Roles = new string[] { "User" }
                };

                await _userRepository.AddAsync(user);
                Console.WriteLine($"New user created: {user.Nombre} - {user.Correo}");
            }
            else
            {
                Console.WriteLine($"User already exists: {user.Nombre} - {user.Correo}");
            }

            return user;
        }

    }
}
