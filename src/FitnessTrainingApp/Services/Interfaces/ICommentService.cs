using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface ICommentService
{
    Task<IReadOnlyList<Comment>> GetForExerciseAsync(int exerciseId);
    Task<IReadOnlyList<Comment>> GetForWorkoutComplexAsync(int workoutComplexId);
    Task<bool> AddAsync(int userId, int exerciseId, string text);
    Task<bool> AddToWorkoutComplexAsync(int userId, int workoutComplexId, string text);
}
