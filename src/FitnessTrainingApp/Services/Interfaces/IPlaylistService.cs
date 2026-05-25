using FitnessTrainingApp.Models.Entities;

namespace FitnessTrainingApp.Services.Interfaces;

public interface IPlaylistService
{
    Task<IReadOnlyList<PlaylistItem>> GetPlaylistAsync(int userId);
    Task<int?> GetExercisePlaylistItemIdAsync(int userId, int exerciseId);
    Task<int?> GetWorkoutComplexPlaylistItemIdAsync(int userId, int workoutComplexId);
    Task<bool> AddExerciseAsync(int userId, int exerciseId);
    Task<bool> AddWorkoutComplexAsync(int userId, int workoutComplexId);
    Task<bool> RemoveAsync(int userId, int playlistItemId);
}
