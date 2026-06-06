using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetUserAsync(int userId);
    Task<bool> RegisterAsync(string fullName, string email, string password);
    Task<bool> UpdateProfileAsync(int userId, string fullName, string email);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> EmailExistsAsync(string email);
}
