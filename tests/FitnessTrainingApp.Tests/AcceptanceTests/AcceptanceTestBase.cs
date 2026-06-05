using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;

namespace FitnessTrainingApp.Tests.AcceptanceTests;

public abstract class AcceptanceTestBase
{
    protected static FitnessTrainingDbContext CreateContext()
    {
        return TestDbContextFactory.CreateContext();
    }

    protected static ExerciseService CreateExerciseService(FitnessTrainingDbContext context)
    {
        return new ExerciseService(context, new TestWebHostEnvironment());
    }

    protected static async Task<User> AddUserAsync(FitnessTrainingDbContext context, int id = 1, UserRole role = UserRole.User)
    {
        var user = TestDataFactory.CreateUser(id, role: role);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    protected static async Task<Exercise> AddExerciseAsync(
        FitnessTrainingDbContext context,
        string name = "Squat",
        DifficultyLevel difficulty = DifficultyLevel.Beginner,
        WorkoutType workoutType = WorkoutType.Home,
        string equipment = "No equipment",
        string muscleGroup = "Legs",
        int trainerId = 1)
    {
        var exercise = TestDataFactory.CreatePublishedExercise(name, difficulty, workoutType, equipment, muscleGroup, trainerId);
        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();
        return exercise;
    }

    protected static async Task<WorkoutComplex> AddComplexAsync(FitnessTrainingDbContext context, int trainerId = 1)
    {
        var complex = TestDataFactory.CreatePublishedWorkoutComplex("Home Workout", trainerId);
        context.WorkoutComplexes.Add(complex);
        await context.SaveChangesAsync();
        return complex;
    }
}
