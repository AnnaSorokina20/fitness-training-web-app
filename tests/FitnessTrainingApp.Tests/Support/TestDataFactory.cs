using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Tests.Support;

public static class TestDataFactory
{
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
}
