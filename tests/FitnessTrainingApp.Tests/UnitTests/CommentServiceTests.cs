using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("CommentService")]
public sealed class CommentServiceTests
{
    [Test]
    public async Task AddAsync_ValidExerciseComment_SavesTrimmedComment()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var result = await new CommentService(context).AddAsync(1, exercise.Id, " Great exercise ");

        Assert.That(result, Is.True);
        Assert.That((await context.Comments.SingleAsync()).Text, Is.EqualTo("Great exercise"));
    }

    [Test]
    public async Task AddAsync_EmptyText_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var result = await new CommentService(context).AddAsync(1, exercise.Id, " ");

        Assert.That(result, Is.False);
        Assert.That(await context.Comments.CountAsync(), Is.Zero);
    }

    [Test]
    public async Task AddToWorkoutComplexAsync_ValidComment_SavesComment()
    {
        using var context = TestDbContextFactory.CreateContext();
        var complex = TestDataFactory.CreatePublishedWorkoutComplex();
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();

        var result = await new CommentService(context).AddToWorkoutComplexAsync(1, complex.Id, "Useful plan");

        Assert.That(result, Is.True);
        Assert.That((await context.Comments.SingleAsync()).WorkoutComplexId, Is.EqualTo(complex.Id));
    }

    [Test]
    public async Task GetForExerciseAsync_ReturnsNewestCommentsFirst()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Users.AddRange(TestDataFactory.CreateUser(1), TestDataFactory.CreateUser(2));
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();
        context.Comments.AddRange(
            new Comment { UserId = 1, ExerciseId = exercise.Id, Text = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
            new Comment { UserId = 2, ExerciseId = exercise.Id, Text = "Second", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var result = await new CommentService(context).GetForExerciseAsync(exercise.Id);

        Assert.That(result.First().Text, Is.EqualTo("Second"));
    }
}
