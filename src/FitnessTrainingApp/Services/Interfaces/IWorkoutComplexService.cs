using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IWorkoutComplexService
{
    Task<IReadOnlyList<WorkoutComplex>> GetAllAsync();
    Task<WorkoutComplex?> GetDetailsAsync(int id);
}
