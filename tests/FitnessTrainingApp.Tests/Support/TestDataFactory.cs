using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Tests.Support;

public static class TestDataFactory
{
    public static User CreateUser(int id = 1, string fullName = "Test User", UserRole role = UserRole.User)
    {
        return new User
        {
            Id = id,
            FullName = fullName,
            Email = $"user{id}@test.local",
            PasswordHash = "hash",
            Role = role
        };
    }

    public static Exercise CreatePublishedExercise(
        string name = "Squat",
        DifficultyLevel difficulty = DifficultyLevel.Beginner,
        WorkoutType workoutType = WorkoutType.Home,
        string equipment = "No equipment",
        string muscleGroup = "Legs",
        int trainerId = 1)
    {
        return new Exercise
        {
            Name = name,
            Description = $"{name} description",
            Difficulty = difficulty,
            WorkoutType = workoutType,
            Equipment = equipment,
            MuscleGroup = muscleGroup,
            SafetyNotes = $"{name} safety notes",
            Status = ContentStatus.Published,
            TrainerId = trainerId,
            MediaFiles =
            [
                new MediaFile
                {
                    FileName = $"{name}.jpg",
                    Url = $"/uploads/exercises/{name}.jpg",
                    ContentType = "image/jpeg"
                }
            ]
        };
    }

    public static WorkoutComplex CreatePublishedWorkoutComplex(
        string name = "Home Starter Plan",
        int trainerId = 1,
        DifficultyLevel difficulty = DifficultyLevel.Beginner,
        WorkoutType workoutType = WorkoutType.Home)
    {
        return new WorkoutComplex
        {
            Name = name,
            Description = $"{name} description",
            Difficulty = difficulty,
            WorkoutType = workoutType,
            DurationMinutes = 30,
            Status = ContentStatus.Published,
            TrainerId = trainerId
        };
    }

    public static WorkoutComplexExercise CreateWorkoutComplexExercise(int exerciseId, int sets = 3, int repetitions = 10)
    {
        return new WorkoutComplexExercise
        {
            ExerciseId = exerciseId,
            Sets = sets,
            Repetitions = repetitions
        };
    }
}
