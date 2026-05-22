using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IModerationService
{
    Task<IReadOnlyList<Exercise>> GetPendingExercisesAsync();
    Task<bool> PublishExerciseAsync(int exerciseId, int adminId);
    Task<bool> RejectExerciseAsync(int exerciseId, int adminId);
}
