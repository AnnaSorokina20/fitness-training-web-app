using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US05")]
public sealed class US05RatingIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task AddRatingToExercise_ShouldPersistRating()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new RatingService(context).AddOrUpdateAsync(user.Id, exercise.Id, 5);

        Assert.That(added, Is.True);
        Assert.That(context.Ratings.Single().Value, Is.EqualTo(5));
    }

    [Test]
    public async Task AddRatingBelowRange_ShouldNotPersistRating()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new RatingService(context).AddOrUpdateAsync(user.Id, exercise.Id, 0);

        Assert.That(added, Is.False);
        Assert.That(context.Ratings.Count(), Is.Zero);
    }

    [Test]
    public async Task CalculateAverageRating_ShouldReturnAverageForExercise()
    {
        using var context = CreateContext();
        var firstUser = await AddUserAsync(context, 1);
        var secondUser = await AddUserAsync(context, 2);
        var exercise = await AddExerciseAsync(context);
        var ratingService = new RatingService(context);
        await ratingService.AddOrUpdateAsync(firstUser.Id, exercise.Id, 5);
        await ratingService.AddOrUpdateAsync(secondUser.Id, exercise.Id, 3);

        var average = await ratingService.CalculateAverageAsync(exercise.Id);

        Assert.That(average, Is.EqualTo(4));
    }

    [Test]
    public async Task AddRatingToWorkoutComplex_ShouldPersistRating()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var complex = await AddWorkoutComplexAsync(context);

        var added = await new RatingService(context).AddOrUpdateWorkoutComplexAsync(user.Id, complex.Id, 4);

        Assert.That(added, Is.True);
        Assert.That(context.Ratings.Single().WorkoutComplexId, Is.EqualTo(complex.Id));
    }
}
