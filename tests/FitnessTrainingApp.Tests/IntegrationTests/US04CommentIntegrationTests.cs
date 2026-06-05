using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US04")]
public sealed class US04CommentIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task AddCommentToExercise_ShouldPersistComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, exercise.Id, "Useful exercise");

        Assert.That(added, Is.True);
        Assert.That(context.Comments.Single().Text, Is.EqualTo("Useful exercise"));
    }

    [Test]
    public async Task AddEmptyComment_ShouldNotPersistComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, exercise.Id, " ");

        Assert.That(added, Is.False);
        Assert.That(context.Comments.Count(), Is.Zero);
    }

    [Test]
    public async Task AddCommentToMissingExercise_ShouldNotPersistComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, 999, "Comment");

        Assert.That(added, Is.False);
        Assert.That(context.Comments.Count(), Is.Zero);
    }

    [Test]
    public async Task AddCommentToWorkoutComplex_ShouldPersistComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var complex = await AddWorkoutComplexAsync(context);

        var added = await new CommentService(context).AddToWorkoutComplexAsync(user.Id, complex.Id, "Useful plan");

        Assert.That(added, Is.True);
        Assert.That(context.Comments.Single().WorkoutComplexId, Is.EqualTo(complex.Id));
    }
}
