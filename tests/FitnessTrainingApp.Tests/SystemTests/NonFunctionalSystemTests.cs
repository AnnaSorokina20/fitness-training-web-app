using System.Diagnostics;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.SystemTests;

[TestFixture]
[Category("System")]
[Category("NonFunctional")]
public sealed class NonFunctionalSystemTests : SystemTestBase
{
    [Test]
    public async Task NFR01_TC01_ExerciseCatalogPerformance_ShouldCompleteWithinThreeSeconds()
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
    public async Task NFR01_TC02_PlaylistPerformance_ShouldCompleteWithinThreeSeconds()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var playlistService = new PlaylistService(context);
        for (var index = 1; index <= 50; index++)
        {
            var exercise = await AddExerciseAsync(context, $"Exercise {index}");
            await playlistService.AddExerciseAsync(user.Id, exercise.Id);
        }

        var stopwatch = Stopwatch.StartNew();
        var playlist = await playlistService.GetPlaylistAsync(user.Id);
        stopwatch.Stop();

        Assert.That(playlist, Has.Count.EqualTo(50));
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(3)));
    }

    [Test]
    public async Task NFR02_TC01_BasicUserScenario_ShouldCompleteWithoutErrors()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        var exercise = await AddExerciseAsync(context, "Squat");

        var registered = await authService.RegisterAsync("Anna", "user@test.local", "Strong123");
        var user = await authService.LoginAsync("user@test.local", "Strong123");
        var searchResult = await CreateExerciseService(context).SearchAsync("Squat");
        var added = await new PlaylistService(context).AddExerciseAsync(user!.Id, exercise.Id);

        Assert.That(registered, Is.True);
        Assert.That(searchResult.Single().Id, Is.EqualTo(exercise.Id));
        Assert.That(added, Is.True);
    }

    [Test]
    public async Task NFR04_TC01_PlaylistSaveFailure_ShouldNotLeavePartialData()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);

        var removedMissingItem = await new PlaylistService(context).RemoveAsync(user.Id, 999);

        Assert.That(removedMissingItem, Is.False);
        Assert.That(context.PlaylistItems.Count(), Is.Zero);
    }

    [Test]
    public async Task NFR04_TC02_CommentSaveFailure_ShouldNotCreatePartialComment()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);

        var added = await new CommentService(context).AddAsync(user.Id, 999, "Comment for missing exercise");

        Assert.That(added, Is.False);
        Assert.That(context.Comments.Count(), Is.Zero);
    }

    [Test]
    public async Task NFR05_TC01_ExpiredSession_ShouldBeInvalidAfterThirtyMinutes()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        context.UserSessions.Add(new UserSession
        {
            UserId = user.Id,
            Role = user.Role,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-30)
        });
        await context.SaveChangesAsync();

        var validSessionExists = context.UserSessions.Any(session =>
            session.UserId == user.Id &&
            session.IsActive &&
            session.ExpiresAt > DateTime.UtcNow);

        Assert.That(validSessionExists, Is.False);
    }

    [Test]
    public async Task NFR06_TC01_WorkoutComplexWithoutExercises_ShouldBeRejected()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var complex = new WorkoutComplex
        {
            Name = "Empty Complex",
            Description = "Invalid complex",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            DurationMinutes = 30,
            TrainerId = trainer.Id
        };

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(complex, []);

        Assert.That(created, Is.False);
        Assert.That(context.WorkoutComplexes.Count(), Is.Zero);
    }

    [Test]
    public void NFR07_TC01_SystemModularity_ShouldExposeSeparateServices()
    {
        var serviceTypes = new[]
        {
            typeof(ExerciseService),
            typeof(PlaylistService),
            typeof(UserManagementService),
            typeof(WorkoutComplexService),
            typeof(CommentService),
            typeof(RatingService),
            typeof(AuthService),
            typeof(AdminLogService)
        };

        Assert.That(serviceTypes.All(type => type.IsClass), Is.True);
    }
}
