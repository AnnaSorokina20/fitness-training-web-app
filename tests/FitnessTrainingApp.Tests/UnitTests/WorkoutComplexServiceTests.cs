using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("WorkoutComplexService")]
public sealed class WorkoutComplexServiceTests
{
    [Test]
    public async Task GetAllAsync_ReturnsOnlyPublishedComplexes()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.WorkoutComplexes.Add(TestDataFactory.CreatePublishedWorkoutComplex("Published Plan"));
        var pendingComplex = TestDataFactory.CreatePublishedWorkoutComplex("Pending Plan");
        pendingComplex.Status = ContentStatus.PendingModeration;
        context.WorkoutComplexes.Add(pendingComplex);
        await context.SaveChangesAsync();

        var result = await new WorkoutComplexService(context).GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Published Plan"));
    }

    [Test]
    public async Task CreateForTrainerAsync_ValidComplex_CreatesPendingComplex()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Trainer Plan");
        var result = await new WorkoutComplexService(context).CreateForTrainerAsync(
            complex,
            [TestDataFactory.CreateWorkoutComplexExercise(exercise.Id)]);

        Assert.That(result, Is.True);
        Assert.That((await context.WorkoutComplexes.SingleAsync()).Status, Is.EqualTo(ContentStatus.PendingModeration));
        Assert.That(await context.WorkoutComplexExercises.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateForTrainerAsync_DuplicateExercises_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Invalid Plan");
        var result = await new WorkoutComplexService(context).CreateForTrainerAsync(
            complex,
            [
                TestDataFactory.CreateWorkoutComplexExercise(exercise.Id),
                TestDataFactory.CreateWorkoutComplexExercise(exercise.Id)
            ]);

        Assert.That(result, Is.False);
        Assert.That(await context.WorkoutComplexes.CountAsync(), Is.Zero);
    }

    [Test]
    public async Task GetAvailableExercisesAsync_ReturnsPublishedExercises()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Published Exercise"));
        var pendingExercise = TestDataFactory.CreatePublishedExercise("Pending Exercise");
        pendingExercise.Status = ContentStatus.PendingModeration;
        context.Exercises.Add(pendingExercise);
        await context.SaveChangesAsync();

        var result = await new WorkoutComplexService(context).GetAvailableExercisesAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Published Exercise"));
    }
}
