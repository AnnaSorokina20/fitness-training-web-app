using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IUserManagementService
{
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<bool> ChangeRoleAsync(int userId, UserRole role, int adminId);
    Task<bool> DeleteUserAsync(int userId, int adminId);
}
