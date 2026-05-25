using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US02")]
public sealed class US02ExerciseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GetAllExercises_ShouldReturnPublishedCatalogFromDatabase()
    {
        using var context = CreateContext();
        for (var index = 1; index <= 5; index++)
        {
            await AddExerciseAsync(context, $"Exercise {index}");
        }

        var exercises = await CreateExerciseService(context).GetAllAsync();

        Assert.That(exercises, Has.Count.EqualTo(5));
    }

    [Test]
    public async Task SearchExercises_ByKeyword_ShouldReturnMatchingExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");
        await AddExerciseAsync(context, "Push Up", muscleGroup: "Chest");

        var exercises = await CreateExerciseService(context).SearchAsync("squat");

        Assert.That(exercises.Single().Name, Is.EqualTo("Squat"));
    }

    [Test]
    public async Task FilterExercises_ByParameters_ShouldReturnMatchingExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Goblet Squat", equipment: "Dumbbell", muscleGroup: "Legs");
        await AddExerciseAsync(context, "Pull Up", DifficultyLevel.Advanced, WorkoutType.Gym, "Bar", "Back");

        var exercises = await CreateExerciseService(context).FilterAsync(DifficultyLevel.Beginner, "Dumbbell", "Legs", null);

        Assert.That(exercises.Single().Name, Is.EqualTo("Goblet Squat"));
    }

    [Test]
    public async Task GetExerciseDetails_ShouldReturnExerciseWithMediaAndDescription()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, "Plank", equipment: "None", muscleGroup: "Core", trainerId: trainer.Id);

        var details = await CreateExerciseService(context).GetDetailsAsync(exercise.Id);

        Assert.That(details, Is.Not.Null);
        Assert.That(details!.Description, Does.Contain("Plank"));
        Assert.That(details.MediaFiles, Is.Not.Empty);
    }

    [Test]
    public async Task CreateExercise_ByTrainer_ShouldPersistPendingExercise()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);

        var created = await CreateExerciseService(context).CreateForTrainerAsync(
            new Exercise
            {
                Name = "Trainer Exercise",
                Description = "Created during integration test",
                Difficulty = DifficultyLevel.Beginner,
                WorkoutType = WorkoutType.Home,
                Equipment = "No equipment",
                MuscleGroup = "Core",
                SafetyNotes = "Keep control",
                TrainerId = trainer.Id
            },
            "https://example.com/trainer-exercise.jpg",
            null);

        Assert.That(created, Is.True);
        Assert.That(context.Exercises.Single(exercise => exercise.Name == "Trainer Exercise").Status, Is.EqualTo(ContentStatus.PendingModeration));
    }
}
