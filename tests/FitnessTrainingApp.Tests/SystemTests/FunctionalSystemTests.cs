using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using System.Diagnostics;

namespace FitnessTrainingApp.Tests.SystemTests;

[TestFixture]
[Category("System")]
[Category("Functional")]
public sealed class FunctionalSystemTests : SystemTestBase
{
    [Test]
    public async Task FR101_TC01_GetExerciseCatalog_ShouldShowAllPublishedExercises()
    {
        using var context = CreateContext();
        for (var index = 1; index <= 10; index++)
        {
            await AddExerciseAsync(context, $"Exercise {index}");
        }

        var exercises = await CreateExerciseService(context).GetAllAsync();

        Assert.That(exercises, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task FR102_TC01_FilterByDifficulty_ShouldReturnOnlyBeginnerExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Beginner Squat", DifficultyLevel.Beginner);
        await AddExerciseAsync(context, "Advanced Pull Up", DifficultyLevel.Advanced, WorkoutType.Gym, "Bar", "Back");

        var exercises = await CreateExerciseService(context).FilterAsync(DifficultyLevel.Beginner, null, null, null);

        Assert.That(exercises, Has.Count.EqualTo(1));
        Assert.That(exercises.All(exercise => exercise.Difficulty == DifficultyLevel.Beginner), Is.True);
    }

    [Test]
    public async Task FR103_TC01_GetExerciseDetails_ShouldReturnFullExerciseInformation()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, "Plank", equipment: "No equipment", muscleGroup: "Core", trainerId: trainer.Id);

        var details = await CreateExerciseService(context).GetDetailsAsync(exercise.Id);

        Assert.That(details, Is.Not.Null);
        Assert.That(details!.Description, Does.Contain("Plank"));
        Assert.That(details.MediaFiles, Is.Not.Empty);
    }

    [Test]
    public async Task FR105_TC01_GetExerciseDetails_ShouldCompleteWithinTwoSeconds()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, "Fast Details", equipment: "No equipment", muscleGroup: "Core", trainerId: trainer.Id);

        var stopwatch = Stopwatch.StartNew();
        var details = await CreateExerciseService(context).GetDetailsAsync(exercise.Id);
        stopwatch.Stop();

        Assert.That(details, Is.Not.Null);
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(2)));
    }

    [Test]
    public async Task FR104_TC01_SearchByKeyword_ShouldReturnRelevantExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");
        await AddExerciseAsync(context, "Push Up", muscleGroup: "Chest");

        var exercises = await CreateExerciseService(context).SearchAsync("squat");

        Assert.That(exercises.Single().Name, Is.EqualTo("Squat"));
    }

    [Test]
    public async Task FR106_TC01_SearchSuggestions_WithTwoSymbols_ShouldReturnSuggestions()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");
        await AddExerciseAsync(context, "Stretch", muscleGroup: "Core");

        var suggestions = await CreateExerciseService(context).GetSuggestionsAsync("Sq");

        Assert.That(suggestions, Does.Contain("Squat"));
    }

    [Test]
    public async Task FR201_TC01_AddExerciseToPlaylist_ShouldSavePlaylistItem()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new PlaylistService(context).AddExerciseAsync(user.Id, exercise.Id);

        Assert.That(added, Is.True);
        Assert.That(context.PlaylistItems.Single().ItemType, Is.EqualTo(PlaylistItemType.Exercise));
    }

    [Test]
    public async Task FR201_TC02_AddDuplicateExerciseToPlaylist_ShouldNotCreateDuplicate()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);
        var playlistService = new PlaylistService(context);

        await playlistService.AddExerciseAsync(user.Id, exercise.Id);
        var addedAgain = await playlistService.AddExerciseAsync(user.Id, exercise.Id);

        Assert.That(addedAgain, Is.False);
        Assert.That(context.PlaylistItems.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task FR203_TC01_AddComment_ShouldSaveComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, exercise.Id, "Good exercise");

        Assert.That(added, Is.True);
        Assert.That(context.Comments.Single().Text, Is.EqualTo("Good exercise"));
    }

    [Test]
    public async Task FR204_TC01_AddRating_ShouldSaveRating()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new RatingService(context).AddOrUpdateAsync(user.Id, exercise.Id, 5);

        Assert.That(added, Is.True);
        Assert.That(context.Ratings.Single().Value, Is.EqualTo(5));
    }

    [Test]
    public async Task FR301_TC01_CreateExerciseByTrainer_ShouldCreateExerciseWithModerationStatus()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);

        var created = await CreateExerciseService(context).CreateForTrainerAsync(
            new Exercise
            {
                Name = "Trainer Exercise",
                Description = "System test exercise",
                Difficulty = DifficultyLevel.Beginner,
                WorkoutType = WorkoutType.Home,
                Equipment = "No equipment",
                MuscleGroup = "Core",
                SafetyNotes = "Keep control",
                TrainerId = trainer.Id
            },
            "https://example.com/system-exercise.jpg",
            null);

        Assert.That(created, Is.True);
        Assert.That(context.Exercises.Single().Status, Is.EqualTo(ContentStatus.PendingModeration));
    }

    [Test]
    public async Task FR303_TC01_CreateWorkoutComplex_ShouldPersistComplex()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, trainerId: trainer.Id);
        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Trainer Complex", trainer.Id);

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(
            complex,
            [TestDataFactory.CreateWorkoutComplexExercise(exercise.Id)]);

        Assert.That(created, Is.True);
        Assert.That(context.WorkoutComplexExercises.Single().ExerciseId, Is.EqualTo(exercise.Id));
    }

    [Test]
    public async Task FR402_TC01_ChangeUserRoleByAdmin_ShouldUpdateRoleAndLogAction()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        var changed = await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, admin.Id);

        Assert.That(changed, Is.True);
        Assert.That(context.Users.Find(user.Id)?.Role, Is.EqualTo(UserRole.Trainer));
        Assert.That(context.AdminLogs.Single().Action, Is.EqualTo("ChangeUserRole"));
    }

    [Test]
    public async Task FR403_TC01_DeleteUserByAdmin_ShouldDeactivateUserAndLogAction()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        var deleted = await new UserManagementService(context).DeleteUserAsync(user.Id, admin.Id);

        Assert.That(deleted, Is.True);
        Assert.That(context.Users.Find(user.Id)?.IsDeleted, Is.True);
        Assert.That(context.AdminLogs.Single().Action, Is.EqualTo("DeleteUser"));
    }

    [Test]
    public async Task FR501_TC01_RegisterUser_ShouldCreateUserWithUserRole()
    {
        using var context = CreateContext();

        var registered = await new AuthService(context).RegisterAsync("Anna", "user@test.local", "Strong123");

        Assert.That(registered, Is.True);
        Assert.That(context.Users.Single().Role, Is.EqualTo(UserRole.User));
    }
}
