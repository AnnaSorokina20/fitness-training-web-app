using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("PlaylistService")]
public sealed class PlaylistServiceTests
{
    [Test]
    public async Task AddExerciseAsync_PublishedExercise_CreatesPlaylistItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var result = await new PlaylistService(context).AddExerciseAsync(1, exercise.Id);

        Assert.That(result, Is.True);
        Assert.That(await context.PlaylistItems.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddExerciseAsync_DuplicateExercise_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var service = new PlaylistService(context);
        await service.AddExerciseAsync(1, exercise.Id);
        var duplicateResult = await service.AddExerciseAsync(1, exercise.Id);

        Assert.That(duplicateResult, Is.False);
        Assert.That(await context.PlaylistItems.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task RemoveAsync_ExistingPlaylistItem_MarksItemAsDeleted()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise();
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        var service = new PlaylistService(context);
        await service.AddExerciseAsync(1, exercise.Id);
        var playlistItemId = await service.GetExercisePlaylistItemIdAsync(1, exercise.Id);

        var result = await service.RemoveAsync(1, playlistItemId!.Value);

        Assert.That(result, Is.True);
        Assert.That(await service.GetPlaylistAsync(1), Is.Empty);
    }

    [Test]
    public async Task AddWorkoutComplexAsync_PublishedComplex_CreatesPlaylistItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        var complex = TestDataFactory.CreatePublishedWorkoutComplex();
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();

        var result = await new PlaylistService(context).AddWorkoutComplexAsync(1, complex.Id);

        Assert.That(result, Is.True);
        Assert.That(await context.PlaylistItems.CountAsync(), Is.EqualTo(1));
    }
}
