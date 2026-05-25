using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IAdminLogService
{
    Task<IReadOnlyList<AdminLog>> GetAllAsync();
}
