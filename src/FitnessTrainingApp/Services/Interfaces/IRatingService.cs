namespace FitnessTrainingApp.Services.Interfaces;

public interface IRatingService
{
    Task<bool> AddOrUpdateAsync(int userId, int exerciseId, int value);
    Task<double> CalculateAverageAsync(int exerciseId);
    Task<int> CountAsync(int exerciseId);
    Task<int?> GetUserRatingAsync(int userId, int exerciseId);
    Task<bool> AddOrUpdateWorkoutComplexAsync(int userId, int workoutComplexId, int value);
    Task<double> CalculateWorkoutComplexAverageAsync(int workoutComplexId);
    Task<int> CountWorkoutComplexAsync(int workoutComplexId);
    Task<int?> GetUserWorkoutComplexRatingAsync(int userId, int workoutComplexId);
}
