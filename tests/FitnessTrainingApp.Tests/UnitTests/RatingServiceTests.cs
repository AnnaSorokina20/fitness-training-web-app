using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("RatingService")]
public sealed class RatingServiceTests
{
    [Test]
    public async Task AddOrUpdateAsync_ValidExerciseRating_SavesRating()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var result = await new RatingService(context).AddOrUpdateAsync(1, exercise.Id, 5);

        Assert.That(result, Is.True);
        Assert.That(await context.Ratings.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddOrUpdateAsync_SecondRatingFromSameUser_UpdatesExistingRating()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var service = new RatingService(context);
        await service.AddOrUpdateAsync(1, exercise.Id, 2);
        await service.AddOrUpdateAsync(1, exercise.Id, 4);

        Assert.That(await context.Ratings.CountAsync(), Is.EqualTo(1));
        Assert.That(await service.GetUserRatingAsync(1, exercise.Id), Is.EqualTo(4));
    }

    [Test]
    public async Task AddOrUpdateAsync_InvalidValue_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var result = await new RatingService(context).AddOrUpdateAsync(1, exercise.Id, 6);

        Assert.That(result, Is.False);
        Assert.That(await context.Ratings.CountAsync(), Is.Zero);
    }

    [Test]
    public async Task CalculateAverageAsync_ReturnsAverageRating()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var service = new RatingService(context);
        await service.AddOrUpdateAsync(1, exercise.Id, 5);
        await service.AddOrUpdateAsync(2, exercise.Id, 3);

        Assert.That(await service.CalculateAverageAsync(exercise.Id), Is.EqualTo(4));
    }

    [Test]
    public async Task AddOrUpdateWorkoutComplexAsync_ValidRating_SavesRating()
    {
        using var context = TestDbContextFactory.CreateContext();
        var complex = TestDataFactory.CreatePublishedWorkoutComplex();
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();

        var result = await new RatingService(context).AddOrUpdateWorkoutComplexAsync(1, complex.Id, 5);

        Assert.That(result, Is.True);
        Assert.That(await context.Ratings.CountAsync(), Is.EqualTo(1));
    }
}
