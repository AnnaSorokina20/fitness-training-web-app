using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class ModerationService(FitnessTrainingDbContext context) : IModerationService
{
    public async Task<IReadOnlyList<Exercise>> GetPendingExercisesAsync()
    {
        return await context.Exercises
            .AsNoTracking()
            .Include(exercise => exercise.Trainer)
            .Where(exercise => !exercise.IsDeleted && exercise.Status == ContentStatus.PendingModeration)
            .OrderBy(exercise => exercise.UpdatedAt ?? exercise.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<WorkoutComplex>> GetPendingWorkoutComplexesAsync()
    {
        return await context.WorkoutComplexes
            .AsNoTracking()
            .Include(complex => complex.Trainer)
            .Include(complex => complex.WorkoutComplexExercises)
            .Where(complex => !complex.IsDeleted && complex.Status == ContentStatus.PendingModeration)
            .OrderBy(complex => complex.UpdatedAt ?? complex.CreatedAt)
            .ToListAsync();
    }

    public async Task<Exercise?> GetPendingExerciseDetailsAsync(int exerciseId)
    {
        return await context.Exercises
            .AsNoTracking()
            .Include(exercise => exercise.Trainer)
            .Include(exercise => exercise.MediaFiles)
            .FirstOrDefaultAsync(exercise =>
                exercise.Id == exerciseId &&
                !exercise.IsDeleted &&
                exercise.Status == ContentStatus.PendingModeration);
    }

    public async Task<WorkoutComplex?> GetPendingWorkoutComplexDetailsAsync(int workoutComplexId)
    {
        return await context.WorkoutComplexes
            .AsNoTracking()
            .Include(complex => complex.Trainer)
            .Include(complex => complex.WorkoutComplexExercises.OrderBy(item => item.OrderNumber))
            .ThenInclude(item => item.Exercise)
            .FirstOrDefaultAsync(complex =>
                complex.Id == workoutComplexId &&
                !complex.IsDeleted &&
                complex.Status == ContentStatus.PendingModeration);
    }

    public async Task<bool> PublishExerciseAsync(int exerciseId, int adminId)
    {
        return await ChangeExerciseStatusAsync(exerciseId, adminId, ContentStatus.Published, "PublishExercise");
    }

    public async Task<bool> RejectExerciseAsync(int exerciseId, int adminId, string? moderationComment)
    {
        return await ChangeExerciseStatusAsync(exerciseId, adminId, ContentStatus.Rejected, "RejectExercise", moderationComment);
    }

    public async Task<bool> PublishWorkoutComplexAsync(int workoutComplexId, int adminId)
    {
        return await ChangeWorkoutComplexStatusAsync(workoutComplexId, adminId, ContentStatus.Published, "PublishWorkoutComplex");
    }

    public async Task<bool> RejectWorkoutComplexAsync(int workoutComplexId, int adminId, string? moderationComment)
    {
        return await ChangeWorkoutComplexStatusAsync(workoutComplexId, adminId, ContentStatus.Rejected, "RejectWorkoutComplex", moderationComment);
    }

    private async Task<bool> ChangeExerciseStatusAsync(int exerciseId, int adminId, ContentStatus status, string action, string? moderationComment = null)
    {
        var exercise = await context.Exercises.FirstOrDefaultAsync(existing =>
            existing.Id == exerciseId &&
            !existing.IsDeleted &&
            existing.Status == ContentStatus.PendingModeration);

        if (exercise is null)
        {
            return false;
        }

        exercise.Status = status;
        exercise.ModerationComment = status == ContentStatus.Rejected ? NormalizeComment(moderationComment) : null;
        exercise.UpdatedAt = DateTime.UtcNow;

        context.AdminLogs.Add(new AdminLog
        {
            AdminId = adminId,
            Action = action,
            EntityName = nameof(Exercise),
            EntityId = exercise.Id
        });

        await context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> ChangeWorkoutComplexStatusAsync(int workoutComplexId, int adminId, ContentStatus status, string action, string? moderationComment = null)
    {
        var complex = await context.WorkoutComplexes.FirstOrDefaultAsync(existing =>
            existing.Id == workoutComplexId &&
            !existing.IsDeleted &&
            existing.Status == ContentStatus.PendingModeration);

        if (complex is null)
        {
            return false;
        }

        complex.Status = status;
        complex.ModerationComment = status == ContentStatus.Rejected ? NormalizeComment(moderationComment) : null;
        complex.UpdatedAt = DateTime.UtcNow;

        context.AdminLogs.Add(new AdminLog
        {
            AdminId = adminId,
            Action = action,
            EntityName = nameof(WorkoutComplex),
            EntityId = complex.Id
        });

        await context.SaveChangesAsync();
        return true;
    }

    private static string? NormalizeComment(string? moderationComment)
    {
        if (string.IsNullOrWhiteSpace(moderationComment))
        {
            return null;
        }

        return moderationComment.Trim();
    }
}
