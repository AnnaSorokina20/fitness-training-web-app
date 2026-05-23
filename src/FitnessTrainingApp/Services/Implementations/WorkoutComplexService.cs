using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class WorkoutComplexService(FitnessTrainingDbContext context) : IWorkoutComplexService
{
    public async Task<IReadOnlyList<WorkoutComplex>> GetAllAsync()
    {
        return await PublishedComplexes()
            .Include(complex => complex.WorkoutComplexExercises)
            .OrderBy(complex => complex.Name)
            .ToListAsync();
    }

    public async Task<WorkoutComplex?> GetDetailsAsync(int id)
    {
        return await PublishedComplexes()
            .Include(complex => complex.WorkoutComplexExercises.OrderBy(item => item.OrderNumber))
            .ThenInclude(item => item.Exercise)
            .FirstOrDefaultAsync(complex => complex.Id == id);
    }

    public async Task<IReadOnlyList<WorkoutComplex>> GetForTrainerAsync(int trainerId)
    {
        return await context.WorkoutComplexes
            .AsNoTracking()
            .Include(complex => complex.WorkoutComplexExercises)
            .Where(complex => complex.TrainerId == trainerId && !complex.IsDeleted)
            .OrderByDescending(complex => complex.UpdatedAt ?? complex.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Exercise>> GetAvailableExercisesAsync()
    {
        return await context.Exercises
            .AsNoTracking()
            .Where(exercise => !exercise.IsDeleted && exercise.Status == ContentStatus.Published)
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateForTrainerAsync(WorkoutComplex complex, IReadOnlyList<WorkoutComplexExercise> exercises)
    {
        return await CreateAsync(complex, exercises, ContentStatus.PendingModeration);
    }

    public async Task<bool> CreatePublishedAsync(WorkoutComplex complex, IReadOnlyList<WorkoutComplexExercise> exercises)
    {
        return await CreateAsync(complex, exercises, ContentStatus.Published);
    }

    private async Task<bool> CreateAsync(WorkoutComplex complex, IReadOnlyList<WorkoutComplexExercise> exercises, ContentStatus status)
    {
        if (!IsValidTrainerComplex(complex, exercises))
        {
            return false;
        }

        var exerciseIds = exercises.Select(item => item.ExerciseId).Distinct().ToList();
        var existingExerciseCount = await context.Exercises.CountAsync(exercise =>
            exerciseIds.Contains(exercise.Id) &&
            !exercise.IsDeleted &&
            exercise.Status == ContentStatus.Published);

        if (existingExerciseCount != exerciseIds.Count)
        {
            return false;
        }

        complex.Status = status;
        complex.ModerationComment = null;
        complex.CreatedAt = DateTime.UtcNow;
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();

        var order = 1;
        foreach (var item in exercises)
        {
            context.WorkoutComplexExercises.Add(new WorkoutComplexExercise
            {
                WorkoutComplexId = complex.Id,
                ExerciseId = item.ExerciseId,
                OrderNumber = order,
                Sets = item.Sets,
                Repetitions = item.Repetitions
            });
            order++;
        }

        await context.SaveChangesAsync();
        return true;
    }

    private IQueryable<WorkoutComplex> PublishedComplexes()
    {
        return context.WorkoutComplexes
            .AsNoTracking()
            .Where(complex => !complex.IsDeleted && complex.Status == ContentStatus.Published);
    }

    private static bool IsValidTrainerComplex(WorkoutComplex complex, IReadOnlyList<WorkoutComplexExercise> exercises)
    {
        return !string.IsNullOrWhiteSpace(complex.Name) &&
               !string.IsNullOrWhiteSpace(complex.Description) &&
               complex.DurationMinutes > 0 &&
               exercises.Count > 0 &&
               exercises.Select(item => item.ExerciseId).Distinct().Count() == exercises.Count &&
               exercises.All(item => item.ExerciseId > 0 && item.Sets > 0 && item.Repetitions > 0);
    }
}
