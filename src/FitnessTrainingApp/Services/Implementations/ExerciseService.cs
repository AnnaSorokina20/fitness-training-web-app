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

    public async Task<IReadOnlyList<Exercise>> GetForTrainerAsync(int trainerId)
    {
        return await context.Exercises
            .AsNoTracking()
            .Where(exercise => exercise.TrainerId == trainerId && !exercise.IsDeleted)
            .OrderByDescending(exercise => exercise.UpdatedAt ?? exercise.CreatedAt)
            .ToListAsync();
    }

    public async Task<Exercise?> GetTrainerExerciseAsync(int id, int trainerId)
    {
        return await context.Exercises
            .AsNoTracking()
            .Include(exercise => exercise.MediaFiles)
            .FirstOrDefaultAsync(exercise => exercise.Id == id && exercise.TrainerId == trainerId && !exercise.IsDeleted);
    }

    public async Task<bool> CreateForTrainerAsync(Exercise exercise, string mediaUrl)
    {
        if (!IsValidTrainerExercise(exercise, mediaUrl))
        {
            return false;
        }

        exercise.Status = ContentStatus.PendingModeration;
        exercise.CreatedAt = DateTime.UtcNow;

        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        context.MediaFiles.Add(CreateMediaFile(exercise.Id, mediaUrl));
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateForTrainerAsync(int id, int trainerId, Exercise exercise, string mediaUrl)
    {
        if (!IsValidTrainerExercise(exercise, mediaUrl))
        {
            return false;
        }

        var existing = await context.Exercises
            .Include(item => item.MediaFiles)
            .FirstOrDefaultAsync(item => item.Id == id && item.TrainerId == trainerId && !item.IsDeleted);

        if (existing is null)
        {
            return false;
        }

        existing.Name = exercise.Name.Trim();
        existing.Description = exercise.Description.Trim();
        existing.Difficulty = exercise.Difficulty;
        existing.WorkoutType = exercise.WorkoutType;
        existing.Equipment = exercise.Equipment.Trim();
        existing.MuscleGroup = exercise.MuscleGroup.Trim();
        existing.SafetyNotes = exercise.SafetyNotes.Trim();
        existing.Status = ContentStatus.PendingModeration;
        existing.UpdatedAt = DateTime.UtcNow;

        var mediaFile = existing.MediaFiles.FirstOrDefault();

        if (mediaFile is null)
        {
            context.MediaFiles.Add(CreateMediaFile(existing.Id, mediaUrl));
        }
        else
        {
            mediaFile.Url = mediaUrl.Trim();
            mediaFile.FileName = Path.GetFileName(mediaUrl.Trim());
            mediaFile.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Exercise> PublishedExercises()
    {
        return context.Exercises
            .AsNoTracking()
            .Where(exercise => !exercise.IsDeleted && exercise.Status == ContentStatus.Published);
    }

    private static bool IsValidTrainerExercise(Exercise exercise, string mediaUrl)
    {
        return !string.IsNullOrWhiteSpace(exercise.Name) &&
               !string.IsNullOrWhiteSpace(exercise.Description) &&
               !string.IsNullOrWhiteSpace(exercise.Equipment) &&
               !string.IsNullOrWhiteSpace(exercise.MuscleGroup) &&
               !string.IsNullOrWhiteSpace(exercise.SafetyNotes) &&
               !string.IsNullOrWhiteSpace(mediaUrl);
    }

    private static MediaFile CreateMediaFile(int exerciseId, string mediaUrl)
    {
        var normalizedUrl = mediaUrl.Trim();

        return new MediaFile
        {
            ExerciseId = exerciseId,
            Url = normalizedUrl,
            FileName = Path.GetFileName(normalizedUrl),
            ContentType = "image/jpeg"
        };
    }
}
