using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IModerationService
{
    Task<IReadOnlyList<Exercise>> GetPendingExercisesAsync();
    Task<IReadOnlyList<WorkoutComplex>> GetPendingWorkoutComplexesAsync();
    Task<bool> PublishExerciseAsync(int exerciseId, int adminId);
    Task<bool> RejectExerciseAsync(int exerciseId, int adminId);
    Task<bool> PublishWorkoutComplexAsync(int workoutComplexId, int adminId);
    Task<bool> RejectWorkoutComplexAsync(int workoutComplexId, int adminId);
}
