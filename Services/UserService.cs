using CineAPI.Repositories.Interfaces;
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
        
        // Obtener usuario por Google ID
        public async Task<Users?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _userRepository.GetByGoogleIdAsync(googleId);
        }
        
        // Agregar usuario con encriptación segura
        public async Task AddUserAsync(Users user)
        {
            // Validar si el Email ya existe (normalizamos a minúsculas si es necesario)
            var existingUser = await _userRepository.GetByEmailAsync(user.Email.ToLowerInvariant());
            if (existingUser != null)
            {
                throw new System.Exception("El Email ya está registrado.");
            }
            
            // Hashear la contraseña antes de agregar
            user.Password = HashPassword(user.Password);
            // Normalizamos el email
            user.Email = user.Email.ToLowerInvariant();
            
            await _userRepository.AddAsync(user);
        }
        
        // Eliminar usuario
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new System.Exception("Usuario no encontrado.");
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
        
        // Actualizar usuario
        public async Task UpdateUserAsync(Users user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.UserID);
            if (existingUser == null)
            {
                throw new System.Exception("Usuario no encontrado.");
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
                    GoogleId = payload.Subject, // ID único de Google
                    PictureUrl = payload.Picture,
                    Roles = new string[] { "User" }
                };
                
                await _userRepository.AddAsync(user);
                Console.WriteLine($"Nuevo usuario creado: {user.Nombre} - {user.Email}");
            }
            else
            {
                Console.WriteLine($"Usuario ya existe: {user.Nombre} - {user.Email}");
            }
            
            return user;
        }
    }
}
