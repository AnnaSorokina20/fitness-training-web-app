using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using FitnessTrainingApp.Services.Interfaces;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class ContentDeletionService(FitnessTrainingDbContext context) : IContentDeletionService
{
    public async Task<bool> DeleteExerciseAsync(int exerciseId, int actorId, bool isAdmin)
    {
        var exercise = await context.Exercises.FirstOrDefaultAsync(existing =>
            existing.Id == exerciseId &&
            !existing.IsDeleted &&
            (isAdmin || existing.TrainerId == actorId));

        if (exercise is null)
        {
            return false;
        }

        exercise.IsDeleted = true;
        exercise.UpdatedAt = DateTime.UtcNow;

        var playlistItems = await context.PlaylistItems
            .Where(item => item.ExerciseId == exerciseId && !item.IsDeleted)
            .ToListAsync();

        foreach (var item in playlistItems)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }

        var affectedComplexIds = await context.WorkoutComplexExercises
            .Where(item => item.ExerciseId == exerciseId)
            .Select(item => item.WorkoutComplexId)
            .Distinct()
            .ToListAsync();

        var links = await context.WorkoutComplexExercises
            .Where(item => item.ExerciseId == exerciseId)
            .ToListAsync();

        context.WorkoutComplexExercises.RemoveRange(links);
        await context.SaveChangesAsync();

        foreach (var complexId in affectedComplexIds)
        {
            await NormalizeComplexAsync(complexId, isAdmin);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWorkoutComplexAsync(int workoutComplexId, int actorId, bool isAdmin)
    {
        var complex = await context.WorkoutComplexes.FirstOrDefaultAsync(existing =>
            existing.Id == workoutComplexId &&
            !existing.IsDeleted &&
            (isAdmin || existing.TrainerId == actorId));

        if (complex is null)
        {
            return false;
        }

        complex.IsDeleted = true;
        complex.UpdatedAt = DateTime.UtcNow;

        var playlistItems = await context.PlaylistItems
            .Where(item => item.WorkoutComplexId == workoutComplexId && !item.IsDeleted)
            .ToListAsync();

        foreach (var item in playlistItems)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }

    private async Task NormalizeComplexAsync(int complexId, bool isAdmin)
    {
        var complex = await context.WorkoutComplexes.FirstOrDefaultAsync(existing =>
            existing.Id == complexId &&
            !existing.IsDeleted);

        if (complex is null)
        {
            return;
        }

        var remainingLinks = await context.WorkoutComplexExercises
            .Where(item => item.WorkoutComplexId == complexId)
            .OrderBy(item => item.OrderNumber)
            .ToListAsync();

        if (remainingLinks.Count == 0)
        {
            complex.IsDeleted = true;
            complex.UpdatedAt = DateTime.UtcNow;
            await DeleteWorkoutComplexPlaylistItemsAsync(complexId);
            return;
        }

        var order = 1;
        foreach (var link in remainingLinks)
        {
            link.OrderNumber = order;
            order++;
        }

        if (!isAdmin && complex.Status == ContentStatus.Published)
        {
            complex.Status = ContentStatus.PendingModeration;
        }

        complex.UpdatedAt = DateTime.UtcNow;
    }

    private async Task DeleteWorkoutComplexPlaylistItemsAsync(int workoutComplexId)
    {
        var playlistItems = await context.PlaylistItems
            .Where(item => item.WorkoutComplexId == workoutComplexId && !item.IsDeleted)
            .ToListAsync();

        foreach (var item in playlistItems)
        {
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
        }
    }
}
