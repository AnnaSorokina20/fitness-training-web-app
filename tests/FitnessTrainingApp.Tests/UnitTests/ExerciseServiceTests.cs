using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("ExerciseService")]
public sealed class ExerciseServiceTests
{
    [Test]
    public async Task GetAllAsync_ReturnsOnlyPublishedExercises()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Squat"));
        var pendingExercise = TestDataFactory.CreatePublishedExercise("Draft Push Up");
        pendingExercise.Status = ContentStatus.PendingModeration;
        context.Exercises.Add(pendingExercise);
        await context.SaveChangesAsync();

        var result = await CreateService(context).GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Squat"));
    }

    [Test]
    public async Task SearchAsync_ByName_ReturnsMatchingExercise()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Bench Press", workoutType: WorkoutType.Gym, equipment: "Barbell", muscleGroup: "Chest"));
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Bodyweight Squat"));
        await context.SaveChangesAsync();

        var result = await CreateService(context).SearchAsync("bench");

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Bench Press"));
    }

    [Test]
    public async Task FilterAsync_ByDifficultyAndWorkoutType_ReturnsMatchingExercises()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Home Squat"));
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Gym Deadlift", DifficultyLevel.Advanced, WorkoutType.Gym, "Barbell", "Back"));
        await context.SaveChangesAsync();

        var result = await CreateService(context).FilterAsync(DifficultyLevel.Advanced, null, null, WorkoutType.Gym);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.Single().Name, Is.EqualTo("Gym Deadlift"));
    }

    [Test]
    public async Task GetSuggestionsAsync_ReturnsRelevantAutocompleteSuggestions()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Bench Press", workoutType: WorkoutType.Gym, equipment: "Barbell", muscleGroup: "Chest"));
        context.Exercises.Add(TestDataFactory.CreatePublishedExercise("Push Up", muscleGroup: "Chest"));
        await context.SaveChangesAsync();

        var result = await CreateService(context).GetSuggestionsAsync("che");

        Assert.That(result, Does.Contain("Chest"));
    }

    [Test]
    public async Task CreateForTrainerAsync_HomeExerciseWithAllowedEquipment_CreatesExercise()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise("Home exercise", equipment: "Bodyweight");

        var created = await CreateService(context).CreateForTrainerAsync(exercise, "https://example.com/home.jpg", null);

        Assert.That(created, Is.True);
        Assert.That(context.Exercises.Single().Equipment, Is.EqualTo("Bodyweight"));
    }

    [Test]
    public async Task CreateForTrainerAsync_HomeExerciseWithForbiddenEquipment_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var exercise = TestDataFactory.CreatePublishedExercise("Invalid home exercise", equipment: "Barbell");

        var created = await CreateService(context).CreateForTrainerAsync(exercise, "https://example.com/home.jpg", null);

        Assert.That(created, Is.False);
        Assert.That(context.Exercises.Count(), Is.Zero);
    }

    private static ExerciseService CreateService(FitnessTrainingApp.Data.FitnessTrainingDbContext context)
    {
        return new ExerciseService(context, new TestWebHostEnvironment());
    }
}
