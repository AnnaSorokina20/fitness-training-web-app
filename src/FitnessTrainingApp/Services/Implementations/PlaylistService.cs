using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class PlaylistService(FitnessTrainingDbContext context) : IPlaylistService
{
    public async Task<IReadOnlyList<PlaylistItem>> GetPlaylistAsync(int userId)
    {
        return await context.PlaylistItems
            .AsNoTracking()
            .Include(item => item.Exercise)
            .Include(item => item.WorkoutComplex)
            .Where(item => item.UserId == userId && !item.IsDeleted)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddExerciseAsync(int userId, int exerciseId)
    {
        var exists = await context.Exercises.AnyAsync(exercise =>
            exercise.Id == exerciseId &&
            !exercise.IsDeleted &&
            exercise.Status == ContentStatus.Published);

        if (!exists)
        {
            return false;
        }

        return await AddAsync(userId, PlaylistItemType.Exercise, exerciseId, null);
    }

    public async Task<bool> AddWorkoutComplexAsync(int userId, int workoutComplexId)
    {
        var exists = await context.WorkoutComplexes.AnyAsync(complex =>
            complex.Id == workoutComplexId &&
            !complex.IsDeleted &&
            complex.Status == ContentStatus.Published);

        if (!exists)
        {
            return false;
        }

        return await AddAsync(userId, PlaylistItemType.WorkoutComplex, null, workoutComplexId);
    }

    public async Task<bool> RemoveAsync(int userId, int playlistItemId)
    {
        var item = await context.PlaylistItems.FirstOrDefaultAsync(existing =>
            existing.Id == playlistItemId &&
            existing.UserId == userId &&
            !existing.IsDeleted);

        if (item is null)
        {
            return false;
        }

        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> AddAsync(int userId, PlaylistItemType itemType, int? exerciseId, int? workoutComplexId)
    {
        var duplicateExists = await context.PlaylistItems.AnyAsync(item =>
            item.UserId == userId &&
            item.ItemType == itemType &&
            item.ExerciseId == exerciseId &&
            item.WorkoutComplexId == workoutComplexId &&
            !item.IsDeleted);

        if (duplicateExists)
        {
            return false;
        }

        context.PlaylistItems.Add(new PlaylistItem
        {
            UserId = userId,
            ItemType = itemType,
            ExerciseId = exerciseId,
            WorkoutComplexId = workoutComplexId
        });

        await context.SaveChangesAsync();
        return true;
    }
}
