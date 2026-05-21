using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FitnessTrainingDbContext>();

        await context.Database.EnsureCreatedAsync();
        await SeedMissingDataAsync(context);
    }

    private static async Task SeedMissingDataAsync(FitnessTrainingDbContext context)
    {
        foreach (var user in SeedData.Users)
        {
            if (!await context.Users.AnyAsync(existing => existing.Id == user.Id))
            {
                context.Users.Add(user);
            }
        }

        foreach (var exercise in SeedData.Exercises)
        {
            if (!await context.Exercises.AnyAsync(existing => existing.Id == exercise.Id))
            {
                context.Exercises.Add(exercise);
            }
        }

        foreach (var mediaFile in SeedData.MediaFiles)
        {
            if (!await context.MediaFiles.AnyAsync(existing => existing.Id == mediaFile.Id))
            {
                context.MediaFiles.Add(mediaFile);
            }
        }

        foreach (var workoutComplex in SeedData.WorkoutComplexes)
        {
            if (!await context.WorkoutComplexes.AnyAsync(existing => existing.Id == workoutComplex.Id))
            {
                context.WorkoutComplexes.Add(workoutComplex);
            }
        }

        foreach (var item in SeedData.WorkoutComplexExercises)
        {
            var exists = await context.WorkoutComplexExercises.AnyAsync(existing =>
                existing.WorkoutComplexId == item.WorkoutComplexId &&
                existing.ExerciseId == item.ExerciseId);

            if (!exists)
            {
                context.WorkoutComplexExercises.Add(item);
            }
        }

        await context.SaveChangesAsync();
    }
}
