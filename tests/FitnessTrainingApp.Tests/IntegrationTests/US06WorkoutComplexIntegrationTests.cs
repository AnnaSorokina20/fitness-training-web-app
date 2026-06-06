using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US06")]
public sealed class US06WorkoutComplexIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task CreateWorkoutComplex_ByTrainer_ShouldPersistComplex()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var exercise = await AddExerciseAsync(context, trainerId: trainer.Id);
        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Trainer Complex", trainer.Id);

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(
            complex,
            [TestDataFactory.CreateWorkoutComplexExercise(exercise.Id)]);

        Assert.That(created, Is.True);
        Assert.That(context.WorkoutComplexes.Single().TrainerId, Is.EqualTo(trainer.Id));
    }

    [Test]
    public async Task CreateWorkoutComplex_WithoutExercises_ShouldNotPersistComplex()
    {
        using var context = CreateContext();
        var trainer = await AddUserAsync(context, role: UserRole.Trainer);
        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Empty Complex", trainer.Id);

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(complex, []);

        Assert.That(created, Is.False);
        Assert.That(context.WorkoutComplexes.Count(), Is.Zero);
    }

    [Test]
    public async Task GetWorkoutComplexDetails_ShouldReturnComplexWithExercises()
    {
        using var context = CreateContext();
        var exercise = await AddExerciseAsync(context);
        var complex = TestDataFactory.CreatePublishedWorkoutComplex();
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();
        var complexExercise = TestDataFactory.CreateWorkoutComplexExercise(exercise.Id);
        complexExercise.WorkoutComplexId = complex.Id;
        complexExercise.OrderNumber = 1;
        context.WorkoutComplexExercises.Add(complexExercise);
        await context.SaveChangesAsync();

        var details = await new WorkoutComplexService(context).GetDetailsAsync(complex.Id);

        Assert.That(details, Is.Not.Null);
        Assert.That(details!.WorkoutComplexExercises.Single().ExerciseId, Is.EqualTo(exercise.Id));
    }

    [Test]
    public async Task AddMissingExerciseToComplex_ShouldReturnFalse()
    {
        using var context = CreateContext();
        var complex = TestDataFactory.CreatePublishedWorkoutComplex();

        var created = await new WorkoutComplexService(context).CreateForTrainerAsync(
            complex,
            [TestDataFactory.CreateWorkoutComplexExercise(999)]);

        Assert.That(created, Is.False);
        Assert.That(context.WorkoutComplexes.Count(), Is.Zero);
    }
}
