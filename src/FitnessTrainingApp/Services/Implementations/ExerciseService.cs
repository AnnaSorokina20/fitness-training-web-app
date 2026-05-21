using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class ExerciseService(FitnessTrainingDbContext context) : IExerciseService
{
    public async Task<IReadOnlyList<Exercise>> GetAllAsync()
    {
        return await PublishedExercises()
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Exercise>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllAsync();
        }

        var normalizedKeyword = keyword.Trim().ToLower();

        return await PublishedExercises()
            .Where(exercise =>
                exercise.Name.ToLower().Contains(normalizedKeyword) ||
                exercise.MuscleGroup.ToLower().Contains(normalizedKeyword) ||
                exercise.Equipment.ToLower().Contains(normalizedKeyword))
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Exercise>> FilterAsync(
        DifficultyLevel? difficulty,
        string? equipment,
        string? muscleGroup,
        WorkoutType? workoutType)
    {
        var query = PublishedExercises();

        if (difficulty.HasValue)
        {
            query = query.Where(exercise => exercise.Difficulty == difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(equipment))
        {
            var normalizedEquipment = equipment.Trim().ToLower();
            query = query.Where(exercise => exercise.Equipment.ToLower().Contains(normalizedEquipment));
        }

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            var normalizedMuscleGroup = muscleGroup.Trim().ToLower();
            query = query.Where(exercise => exercise.MuscleGroup.ToLower().Contains(normalizedMuscleGroup));
        }

        if (workoutType.HasValue)
        {
            query = query.Where(exercise => exercise.WorkoutType == workoutType.Value);
        }

        return await query
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetSuggestionsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var normalizedQuery = query.Trim().ToLower();

        return await PublishedExercises()
            .Where(exercise => exercise.Name.ToLower().Contains(normalizedQuery))
            .OrderBy(exercise => exercise.Name)
            .Select(exercise => exercise.Name)
            .Take(8)
            .ToListAsync();
    }

    public async Task<Exercise?> GetDetailsAsync(int id)
    {
        return await PublishedExercises()
            .Include(exercise => exercise.MediaFiles)
            .Include(exercise => exercise.Comments)
            .Include(exercise => exercise.Ratings)
            .FirstOrDefaultAsync(exercise => exercise.Id == id);
    }

    private IQueryable<Exercise> PublishedExercises()
    {
        return context.Exercises
            .AsNoTracking()
            .Where(exercise => !exercise.IsDeleted && exercise.Status == ContentStatus.Published);
    }
}
