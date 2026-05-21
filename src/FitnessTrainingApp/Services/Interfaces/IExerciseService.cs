using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IExerciseService
{
    Task<IReadOnlyList<Exercise>> GetAllAsync();
    Task<IReadOnlyList<Exercise>> SearchAsync(string keyword);
    Task<IReadOnlyList<Exercise>> FilterAsync(DifficultyLevel? difficulty, string? equipment, string? muscleGroup, WorkoutType? workoutType);
    Task<Exercise?> GetDetailsAsync(int id);
}
