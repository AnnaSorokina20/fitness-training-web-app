namespace FitnessTrainingApp.Services.Interfaces;

public interface IRatingService
{
    Task<bool> AddOrUpdateAsync(int userId, int exerciseId, int value);
    Task<double> CalculateAverageAsync(int exerciseId);
}
