using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US03")]
public sealed class US03PlaylistIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task AddExerciseToPlaylist_ShouldCreatePlaylistItem()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new PlaylistService(context).AddExerciseAsync(user.Id, exercise.Id);

        Assert.That(added, Is.True);
        Assert.That(context.PlaylistItems.Single().ItemType, Is.EqualTo(PlaylistItemType.Exercise));
    }

    [Test]
    public async Task AddWorkoutComplexToPlaylist_ShouldCreatePlaylistItem()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var complex = await AddWorkoutComplexAsync(context);

        var added = await new PlaylistService(context).AddWorkoutComplexAsync(user.Id, complex.Id);

        Assert.That(added, Is.True);
        Assert.That(context.PlaylistItems.Single().ItemType, Is.EqualTo(PlaylistItemType.WorkoutComplex));
    }

    [Test]
    public async Task DuplicatePlaylistItem_ShouldNotBeCreated()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);
        var playlistService = new PlaylistService(context);

        await playlistService.AddExerciseAsync(user.Id, exercise.Id);
        var duplicate = await playlistService.AddExerciseAsync(user.Id, exercise.Id);

        Assert.That(duplicate, Is.False);
        Assert.That(context.PlaylistItems.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task RemoveFromPlaylist_ShouldHidePlaylistItem()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);
        var playlistService = new PlaylistService(context);
        await playlistService.AddExerciseAsync(user.Id, exercise.Id);
        var itemId = await playlistService.GetExercisePlaylistItemIdAsync(user.Id, exercise.Id);

        var removed = await playlistService.RemoveAsync(user.Id, itemId!.Value);

        Assert.That(removed, Is.True);
        Assert.That(await playlistService.GetPlaylistAsync(user.Id), Is.Empty);
    }
}
