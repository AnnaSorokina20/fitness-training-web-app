using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class RatingService(FitnessTrainingDbContext context) : IRatingService
{
    public async Task<bool> AddOrUpdateAsync(int userId, int exerciseId, int value)
    {
        if (value is < 1 or > 5)
        {
            return false;
        }

        var exerciseExists = await context.Exercises.AnyAsync(exercise =>
            exercise.Id == exerciseId &&
            !exercise.IsDeleted &&
            exercise.Status == ContentStatus.Published);

        if (!exerciseExists)
        {
            return false;
        }

        var rating = await context.Ratings.FirstOrDefaultAsync(existing =>
            existing.UserId == userId &&
            existing.ExerciseId == exerciseId &&
            !existing.IsDeleted);

        if (rating is null)
        {
            context.Ratings.Add(new Rating
            {
                UserId = userId,
                ExerciseId = exerciseId,
                Value = value
            });
        }
        else
        {
            rating.Value = value;
            rating.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<double> CalculateAverageAsync(int exerciseId)
    {
        var ratings = context.Ratings
            .AsNoTracking()
            .Where(rating => rating.ExerciseId == exerciseId && !rating.IsDeleted);

        if (!await ratings.AnyAsync())
        {
            return 0;
        }

        return await ratings.AverageAsync(rating => rating.Value);
    }

    public async Task<int> CountAsync(int exerciseId)
    {
        return await context.Ratings
            .AsNoTracking()
            .CountAsync(rating => rating.ExerciseId == exerciseId && !rating.IsDeleted);
    }

    public async Task<int?> GetUserRatingAsync(int userId, int exerciseId)
    {
        return await context.Ratings
            .AsNoTracking()
            .Where(rating => rating.UserId == userId && rating.ExerciseId == exerciseId && !rating.IsDeleted)
            .Select(rating => (int?)rating.Value)
            .FirstOrDefaultAsync();
    }
}
