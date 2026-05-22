using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IWorkoutComplexService
{
    Task<IReadOnlyList<WorkoutComplex>> GetAllAsync();
    Task<WorkoutComplex?> GetDetailsAsync(int id);
    Task<IReadOnlyList<WorkoutComplex>> GetForTrainerAsync(int trainerId);
    Task<IReadOnlyList<Exercise>> GetAvailableExercisesAsync();
    Task<bool> CreateForTrainerAsync(WorkoutComplex complex, IReadOnlyList<WorkoutComplexExercise> exercises);
}
