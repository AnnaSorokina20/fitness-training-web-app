using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface ICommentService
{
    Task<IReadOnlyList<Comment>> GetForExerciseAsync(int exerciseId);
    Task<bool> AddAsync(int userId, int exerciseId, string text);
}
