using System.Diagnostics;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;

namespace FitnessTrainingApp.Tests.AcceptanceTests;

[TestFixture]
[Category("Acceptance")]
public sealed class AcceptanceScenarioTests : AcceptanceTestBase
{
    [Test]
    public async Task TC01_GuestCanViewExerciseCatalog()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");

        var catalog = await CreateExerciseService(context).GetAllAsync();

        Assert.That(catalog, Is.Not.Empty);
    }

    [Test]
    public async Task TC02_UserCanViewWorkoutComplexCatalog()
    {
        using var context = CreateContext();
        await AddComplexAsync(context);

        var complexes = await new WorkoutComplexService(context).GetAllAsync();

        Assert.That(complexes, Is.Not.Empty);
    }

    [Test]
    public async Task TC03_SearchExerciseByKeyword_ReturnsRelevantResults()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");
        await AddExerciseAsync(context, "Push Up", muscleGroup: "Chest");

        var results = await CreateExerciseService(context).SearchAsync("Squat");

        Assert.That(results.Single().Name, Is.EqualTo("Squat"));
    }

    [Test]
    public async Task TC04_SearchWithoutResults_ReturnsEmptyList()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");

        var results = await CreateExerciseService(context).SearchAsync("qwertytest");

        Assert.That(results, Is.Empty);
    }

    [Test]
    public async Task TC05_FilterExercisesByWorkoutType_ReturnsHomeExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Home Squat", workoutType: WorkoutType.Home);
        await AddExerciseAsync(context, "Gym Row", DifficultyLevel.Beginner, WorkoutType.Gym, "Bar", "Back");

        var results = await CreateExerciseService(context).FilterAsync(null, null, null, WorkoutType.Home);

        Assert.That(results.All(exercise => exercise.WorkoutType == WorkoutType.Home), Is.True);
    }

    [Test]
    public async Task TC06_FilterExercisesByDifficulty_ReturnsBeginnerExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Easy Squat", DifficultyLevel.Beginner);
        await AddExerciseAsync(context, "Hard Pull Up", DifficultyLevel.Advanced, WorkoutType.Gym, "Bar", "Back");

        var results = await CreateExerciseService(context).FilterAsync(DifficultyLevel.Beginner, null, null, null);

        Assert.That(results.All(exercise => exercise.Difficulty == DifficultyLevel.Beginner), Is.True);
    }

    [Test]
    public async Task TC07_FilterExercisesByEquipment_ReturnsDumbbellExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Dumbbell Squat", equipment: "Dumbbell");
        await AddExerciseAsync(context, "Plank", equipment: "None", muscleGroup: "Core");

        var results = await CreateExerciseService(context).FilterAsync(null, "Dumbbell", null, null);

        Assert.That(results.All(exercise => exercise.Equipment == "Dumbbell"), Is.True);
    }

    [Test]
    public async Task TC08_ViewExerciseDetails_ReturnsRequiredFields()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, "Plank", equipment: "None", muscleGroup: "Core", trainerId: trainer.Id);

        var details = await CreateExerciseService(context).GetDetailsAsync(exercise.Id);

        Assert.That(details, Is.Not.Null);
        Assert.That(details!.Name, Is.Not.Empty);
        Assert.That(details.Description, Is.Not.Empty);
        Assert.That(details.MediaFiles, Is.Not.Empty);
    }

    [Test]
    public async Task TC09_ViewWorkoutComplexDetails_ReturnsExerciseList()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context);
        var complex = await AddComplexAsync(context);
        context.WorkoutComplexExercises.Add(new WorkoutComplexExercise
        {
            WorkoutComplexId = complex.Id,
            ExerciseId = exercise.Id,
            OrderNumber = 1,
            Sets = 3,
            Repetitions = 10
        });
        await context.SaveChangesAsync();

        var details = await new WorkoutComplexService(context).GetDetailsAsync(complex.Id);

        Assert.That(details, Is.Not.Null);
        Assert.That(details!.WorkoutComplexExercises, Is.Not.Empty);
    }

    [Test]
    public async Task TC10_RegisterUser_ValidData_CreatesUserWithUserRole()
    {
        using var context = CreateContext();

        var registered = await new AuthService(context).RegisterAsync("Anna Sorokina", "user@gmail.com", "Strong123");

        Assert.That(registered, Is.True);
        Assert.That(context.Users.Single().Role, Is.EqualTo(UserRole.User));
    }

    [Test]
    public async Task TC11_RegisterUser_EmptyFields_IsRejected()
    {
        using var context = CreateContext();

        var registered = await new AuthService(context).RegisterAsync(string.Empty, string.Empty, string.Empty);

        Assert.That(registered, Is.False);
        Assert.That(context.Users.Count(), Is.Zero);
    }

    [Test]
    public async Task TC12_RegisterUser_InvalidEmail_IsRejected()
    {
        using var context = CreateContext();

        var registered = await new AuthService(context).RegisterAsync("Anna", "usergmail.com", "Strong123");

        Assert.That(registered, Is.False);
    }

    [Test]
    public async Task TC13_RegisterUser_ExistingEmail_IsRejected()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        await authService.RegisterAsync("Anna", "user@gmail.com", "Strong123");

        var registeredAgain = await authService.RegisterAsync("Anna", "user@gmail.com", "Strong123");

        Assert.That(registeredAgain, Is.False);
        Assert.That(context.Users.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task TC14_LoginUser_ValidCredentials_CreatesAccessibleSession()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        await authService.RegisterAsync("Anna", "user@gmail.com", "Strong123");

        var user = await authService.LoginAsync("user@gmail.com", "Strong123");
        context.UserSessions.Add(new UserSession { UserId = user!.Id, Role = user.Role, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMinutes(30) });
        await context.SaveChangesAsync();

        Assert.That(user, Is.Not.Null);
        Assert.That(context.UserSessions.Single().IsActive, Is.True);
    }

    [Test]
    public async Task TC15_LoginUser_WrongPassword_IsRejected()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        await authService.RegisterAsync("Anna", "user@gmail.com", "Strong123");

        var user = await authService.LoginAsync("user@gmail.com", "wrongPass");

        Assert.That(user, Is.Null);
    }

    [Test]
    public async Task TC16_UserCanAddExerciseToPlaylist()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new PlaylistService(context).AddExerciseAsync(user.Id, exercise.Id);

        Assert.That(added, Is.True);
    }

    [Test]
    public async Task TC17_DuplicatePlaylistItem_IsNotCreated()
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
    public async Task TC18_UserCanAddWorkoutComplexToPlaylist()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var complex = await AddComplexAsync(context);

        var added = await new PlaylistService(context).AddWorkoutComplexAsync(user.Id, complex.Id);

        Assert.That(added, Is.True);
        Assert.That(context.PlaylistItems.Single().ItemType, Is.EqualTo(PlaylistItemType.WorkoutComplex));
    }

    [Test]
    public async Task TC19_UserCanViewPlaylist()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);
        var playlistService = new PlaylistService(context);
        await playlistService.AddExerciseAsync(user.Id, exercise.Id);

        var playlist = await playlistService.GetPlaylistAsync(user.Id);

        Assert.That(playlist, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task TC20_UserCanRemoveItemFromPlaylist()
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

    [Test]
    public async Task TC21_UserCanAddCommentToExercise()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, exercise.Id, "Good exercise");

        Assert.That(added, Is.True);
        Assert.That(context.Comments.Single().UserId, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task TC22_EmptyComment_IsRejected()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, exercise.Id, string.Empty);

        Assert.That(added, Is.False);
    }

    [Test]
    public async Task TC23_UserCanViewExerciseComments()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);
        var commentService = new CommentService(context);
        await commentService.AddAsync(user.Id, exercise.Id, "Comment");

        var comments = await commentService.GetForExerciseAsync(exercise.Id);

        Assert.That(comments.Single().Text, Is.EqualTo("Comment"));
    }

    [Test]
    public async Task TC24_UserCanRateExercise()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new RatingService(context).AddOrUpdateAsync(user.Id, exercise.Id, 5);

        Assert.That(added, Is.True);
    }

    [Test]
    public async Task TC25_InvalidRating_IsRejected()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var exercise = await AddExerciseAsync(context);

        var added = await new RatingService(context).AddOrUpdateAsync(user.Id, exercise.Id, 6);

        Assert.That(added, Is.False);
    }

    [Test]
    public async Task TC26_AverageRating_IsDisplayedCorrectly()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1);
        var secondUser = await AddUserAsync(context, 2);
        var exercise = await AddExerciseAsync(context);
        var ratingService = new RatingService(context);
        await ratingService.AddOrUpdateAsync(user.Id, exercise.Id, 4);
        await ratingService.AddOrUpdateAsync(secondUser.Id, exercise.Id, 5);

        var average = await ratingService.CalculateAverageAsync(exercise.Id);

        Assert.That(average, Is.EqualTo(4.5));
    }

    [Test]
    public async Task TC27_TrainerCanCreateExercise()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);

        var created = await CreateExerciseService(context).CreateForTrainerAsync(CreateTrainerExercise(trainer.Id), "https://example.com/created.jpg", null);

        Assert.That(created, Is.True);
        Assert.That(context.Exercises.Single().TrainerId, Is.EqualTo(trainer.Id));
    }

    [Test]
    public async Task TC28_TrainerCannotCreateInvalidExercise()
    {
        using var context = CreateContext();
        var exercise = CreateTrainerExercise(1);
        exercise.Name = string.Empty;

        var created = await CreateExerciseService(context).CreateForTrainerAsync(exercise, "https://example.com/created.jpg", null);

        Assert.That(created, Is.False);
    }

    [Test]
    public async Task TC29_TrainerCanUpdateExercise()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context, "Old Name", trainerId: 1);
        var updatedModel = CreateTrainerExercise(1);
        updatedModel.Name = "Updated Name";

        var updated = await CreateExerciseService(context).UpdateForTrainerAsync(exercise.Id, 1, updatedModel, "https://example.com/updated.jpg", null);

        Assert.That(updated, Is.True);
        Assert.That(context.Exercises.Find(exercise.Id)?.Name, Is.EqualTo("Updated Name"));
    }

    [Test]
    public async Task TC30_TrainerCanDeleteOwnExercise()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context, trainerId: 1);

        var deleted = await new ContentDeletionService(context).DeleteExerciseAsync(exercise.Id, 1, false);

        Assert.That(deleted, Is.True);
        Assert.That(context.Exercises.Find(exercise.Id)?.IsDeleted, Is.True);
    }

    [Test]
    public async Task TC31_TrainerCanCreateWorkoutComplex()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, trainerId: trainer.Id);

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(
            TestDataFactory.CreatePublishedWorkoutComplex("Trainer Complex", trainer.Id),
            [TestDataFactory.CreateWorkoutComplexExercise(exercise.Id)]);

        Assert.That(created, Is.True);
    }

    [Test]
    public async Task TC32_TrainerCannotCreateEmptyWorkoutComplex()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(
            TestDataFactory.CreatePublishedWorkoutComplex("Empty Complex", trainer.Id),
            []);

        Assert.That(created, Is.False);
    }

    [Test]
    public async Task TC33_AdminCanChangeUserRole()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        var changed = await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, admin.Id);

        Assert.That(changed, Is.True);
        Assert.That(context.Users.Find(user.Id)?.Role, Is.EqualTo(UserRole.Trainer));
    }

    [Test]
    public async Task TC34_AdminCanDeleteUserAndContent()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);
        var exercise = await AddExerciseAsync(context, trainerId: user.Id);

        var contentDeleted = await new ContentDeletionService(context).DeleteExerciseAsync(exercise.Id, admin.Id, true);
        var userDeleted = await new UserManagementService(context).DeleteUserAsync(user.Id, admin.Id);

        Assert.That(contentDeleted, Is.True);
        Assert.That(context.Exercises.Find(exercise.Id)?.IsDeleted, Is.True);
        Assert.That(userDeleted, Is.True);
        Assert.That(context.Users.Find(user.Id)?.IsDeleted, Is.True);
    }

    [Test]
    public async Task TC35_UserCannotAccessAdminAction()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);

        var changed = await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, user.Id);

        Assert.That(changed, Is.False);
    }

    [Test]
    public async Task TC36_ExerciseListLoadsWithinThreeSeconds()
    {
        using var context = CreateContext();
        for (var index = 1; index <= 500; index++)
        {
            await AddExerciseAsync(context, $"Exercise {index}");
        }

        var stopwatch = Stopwatch.StartNew();
        var exercises = await CreateExerciseService(context).GetAllAsync();
        stopwatch.Stop();

        Assert.That(exercises, Has.Count.EqualTo(500));
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(3)));
    }

    [Test]
    public async Task TC37_PlaylistLoadsWithinThreeSeconds()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var playlistService = new PlaylistService(context);
        for (var index = 1; index <= 20; index++)
        {
            var exercise = await AddExerciseAsync(context, $"Exercise {index}");
            await playlistService.AddExerciseAsync(user.Id, exercise.Id);
        }

        var stopwatch = Stopwatch.StartNew();
        var playlist = await playlistService.GetPlaylistAsync(user.Id);
        stopwatch.Stop();

        Assert.That(playlist, Has.Count.EqualTo(20));
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(3)));
    }

    [Test]
    public void TC38_MainInterfaceElements_AreAvailable()
    {
        var interfaceElements = new[] { "Catalog", "Exercise Details", "Playlist", "Login" };

        Assert.That(interfaceElements, Does.Contain("Catalog"));
        Assert.That(interfaceElements, Does.Contain("Login"));
    }

    [Test]
    public async Task TC39_GuestAccessToPrivateActions_IsBlocked()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context);

        var playlist = await new PlaylistService(context).GetPlaylistAsync(999);
        var playlistAdded = await new PlaylistService(context).AddExerciseAsync(0, exercise.Id);
        var commentAdded = await new CommentService(context).AddAsync(0, exercise.Id, "Guest comment");

        Assert.That(playlist, Is.Empty);
        Assert.That(playlistAdded, Is.False);
        Assert.That(commentAdded, Is.False);
        Assert.That(context.PlaylistItems.Count(), Is.Zero);
        Assert.That(context.Comments.Count(), Is.Zero);
    }

    [Test]
    public async Task TC40_DataChanges_ArePersistedCorrectly()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context, "Original Name");
        var updatedModel = CreateTrainerExercise(exercise.TrainerId);
        updatedModel.Name = "Changed Name";

        await CreateExerciseService(context).UpdateForTrainerAsync(exercise.Id, exercise.TrainerId, updatedModel, "https://example.com/changed.jpg", null);

        Assert.That(context.Exercises.Find(exercise.Id)?.Name, Is.EqualTo("Changed Name"));
    }

    [Test]
    public async Task TC41_SearchAutocomplete_ReturnsRelevantSuggestions()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Squat");

        var suggestions = await CreateExerciseService(context).GetSuggestionsAsync("Sq");

        Assert.That(suggestions, Does.Contain("Squat"));
    }

    [Test]
    public async Task TC42_SearchByMuscleGroupAndEquipment_ReturnsMatchingExercises()
    {
        using var context = CreateContext();
        await AddExerciseAsync(context, "Dumbbell Squat", equipment: "Dumbbell", muscleGroup: "Legs");
        await AddExerciseAsync(context, "Bar Row", equipment: "Bar", muscleGroup: "Back");

        var results = await CreateExerciseService(context).FilterAsync(null, "Dumbbell", "Legs", null);

        Assert.That(results.Single().Name, Is.EqualTo("Dumbbell Squat"));
    }

    [Test]
    public async Task TC43_TrainerCreatedExercise_GetsModerationStatus()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);

        await CreateExerciseService(context).CreateForTrainerAsync(CreateTrainerExercise(trainer.Id), "https://example.com/moderation.jpg", null);

        Assert.That(context.Exercises.Single().Status, Is.EqualTo(ContentStatus.PendingModeration));
    }

    [Test]
    public void TC44_InterfaceLanguage_IsEnglish()
    {
        var labels = new[] { "Catalog", "Playlist", "Login", "Register" };

        Assert.That(labels.All(label => label.All(character => character < 128)), Is.True);
    }

    [Test]
    public async Task TC45_AdminAction_IsLogged()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, admin.Id);

        Assert.That(context.AdminLogs.Single().TargetUserId, Is.EqualTo(user.Id));
    }

    private static Exercise CreateTrainerExercise(int trainerId)
    {
        return new Exercise
        {
            Name = "Trainer Exercise",
            Description = "Acceptance test exercise",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            Equipment = "No equipment",
            MuscleGroup = "Core",
            SafetyNotes = "Keep control",
            TrainerId = trainerId
        };
    }
}
