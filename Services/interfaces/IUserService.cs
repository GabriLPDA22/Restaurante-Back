using CineAPI.Models.DTOs;
using Google.Apis.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CineAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users?> GetUserByIdAsync(int userId);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<Users?> GetUserByGoogleIdAsync(string googleId);
        Task AddUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task UpdateUserBasicInfo(int userId, UserUpdateDto updateDto);
        Task DeleteUserAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<Users> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
    }
}