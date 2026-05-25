namespace FitnessTrainingApp.Services.Interfaces;

public interface IContentDeletionService
{
    Task<bool> DeleteExerciseAsync(int exerciseId, int actorId, bool isAdmin);
    Task<bool> DeleteWorkoutComplexAsync(int workoutComplexId, int actorId, bool isAdmin);
}
