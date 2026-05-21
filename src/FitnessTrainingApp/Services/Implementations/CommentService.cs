using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class CommentService(FitnessTrainingDbContext context) : ICommentService
{
    public async Task<IReadOnlyList<Comment>> GetForExerciseAsync(int exerciseId)
    {
        return await context.Comments
            .AsNoTracking()
            .Include(comment => comment.User)
            .Where(comment => comment.ExerciseId == exerciseId && !comment.IsDeleted)
            .OrderByDescending(comment => comment.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(int userId, int exerciseId, string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Trim().Length > 1000)
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

        context.Comments.Add(new Comment
        {
            UserId = userId,
            ExerciseId = exerciseId,
            Text = text.Trim()
        });

        await context.SaveChangesAsync();
        return true;
    }
}
