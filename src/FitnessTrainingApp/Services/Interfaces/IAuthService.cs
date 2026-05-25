using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string fullName, string email, string password);
    Task<bool> EmailExistsAsync(string email);
}
