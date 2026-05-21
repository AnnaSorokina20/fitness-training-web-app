using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Infrastructure.Security;

namespace FitnessTrainingApp.Data;

public static class SeedData
{
    private static readonly DateTime CreatedAt = new(2026, 5, 21, 0, 0, 0, DateTimeKind.Utc);
    private static readonly byte[] AdminSalt = Convert.FromBase64String("YWRtaW4tc2VlZC1zYWx0IQ==");
    private static readonly byte[] TrainerSalt = Convert.FromBase64String("dHJhaW5lci1zZWVkLTEhIQ==");
    private static readonly byte[] UserSalt = Convert.FromBase64String("dXNlci1zZWVkLXNhbHQhIQ==");

    public static IReadOnlyList<User> Users =>
    [
        new()
        {
            Id = 1,
            FullName = "System Administrator",
            Email = "admin@fit.local",
            PasswordHash = PasswordHasher.HashPassword("Admin12345", AdminSalt),
            Role = UserRole.Administrator,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 2,
            FullName = "Olena Trainer",
            Email = "trainer@fit.local",
            PasswordHash = PasswordHasher.HashPassword("Trainer12345", TrainerSalt),
            Role = UserRole.Trainer,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 3,
            FullName = "Demo User",
            Email = "user@fit.local",
            PasswordHash = PasswordHasher.HashPassword("User12345", UserSalt),
            Role = UserRole.User,
            CreatedAt = CreatedAt
        }
    ];

    public static IReadOnlyList<Exercise> Exercises =>
    [
        new()
        {
            Id = 1,
            Name = "Bodyweight Squat",
            Description = "A basic lower-body exercise for strengthening legs and glutes without equipment.",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            Equipment = "No equipment",
            MuscleGroup = "Legs",
            SafetyNotes = "Keep your back neutral and knees aligned with your toes.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 2,
            Name = "Plank",
            Description = "A core stability exercise focused on abdominal and shoulder endurance.",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            Equipment = "Mat",
            MuscleGroup = "Core",
            SafetyNotes = "Do not let your lower back sag during the hold.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 3,
            Name = "Dumbbell Row",
            Description = "A pulling exercise for back and arm strength using dumbbells.",
            Difficulty = DifficultyLevel.Intermediate,
            WorkoutType = WorkoutType.Gym,
            Equipment = "Dumbbells",
            MuscleGroup = "Back",
            SafetyNotes = "Control the weight and avoid twisting your torso.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 4,
            Name = "Bench Press",
            Description = "A compound upper-body exercise for chest, shoulders and triceps.",
            Difficulty = DifficultyLevel.Intermediate,
            WorkoutType = WorkoutType.Gym,
            Equipment = "Barbell",
            MuscleGroup = "Chest",
            SafetyNotes = "Use a spotter when lifting heavy weights.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 5,
            Name = "Deadlift",
            Description = "A full-body strength exercise that develops posterior chain power.",
            Difficulty = DifficultyLevel.Advanced,
            WorkoutType = WorkoutType.Gym,
            Equipment = "Barbell",
            MuscleGroup = "Back",
            SafetyNotes = "Brace your core and keep the bar close to your body.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 6,
            Name = "Push-up",
            Description = "A bodyweight pushing exercise for chest, shoulders and triceps.",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            Equipment = "No equipment",
            MuscleGroup = "Chest",
            SafetyNotes = "Keep your body in a straight line throughout the movement.",
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        }
    ];

    public static IReadOnlyList<MediaFile> MediaFiles =>
    [
        new() { Id = 1, ExerciseId = 1, FileName = "bodyweight-squat.jpg", Url = "/images/exercises/bodyweight-squat.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt },
        new() { Id = 2, ExerciseId = 2, FileName = "plank.jpg", Url = "/images/exercises/plank.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt },
        new() { Id = 3, ExerciseId = 3, FileName = "dumbbell-row.jpg", Url = "/images/exercises/dumbbell-row.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt },
        new() { Id = 4, ExerciseId = 4, FileName = "bench-press.jpg", Url = "/images/exercises/bench-press.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt },
        new() { Id = 5, ExerciseId = 5, FileName = "deadlift.jpg", Url = "/images/exercises/deadlift.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt },
        new() { Id = 6, ExerciseId = 6, FileName = "push-up.jpg", Url = "/images/exercises/push-up.jpg", ContentType = "image/jpeg", CreatedAt = CreatedAt }
    ];

    public static IReadOnlyList<WorkoutComplex> WorkoutComplexes =>
    [
        new()
        {
            Id = 1,
            Name = "Home Starter Plan",
            Description = "A simple beginner-friendly home workout for building consistency without gym equipment.",
            Difficulty = DifficultyLevel.Beginner,
            WorkoutType = WorkoutType.Home,
            DurationMinutes = 25,
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 2,
            Name = "Gym Strength Base",
            Description = "A structured gym workout focused on foundational compound strength exercises.",
            Difficulty = DifficultyLevel.Intermediate,
            WorkoutType = WorkoutType.Gym,
            DurationMinutes = 50,
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        },
        new()
        {
            Id = 3,
            Name = "Upper Body Push",
            Description = "A focused chest and triceps workout built around pushing movements.",
            Difficulty = DifficultyLevel.Intermediate,
            WorkoutType = WorkoutType.Gym,
            DurationMinutes = 40,
            Status = ContentStatus.Published,
            TrainerId = 2,
            CreatedAt = CreatedAt
        }
    ];

    public static IReadOnlyList<WorkoutComplexExercise> WorkoutComplexExercises =>
    [
        new() { WorkoutComplexId = 1, ExerciseId = 1, OrderNumber = 1, Sets = 3, Repetitions = 12 },
        new() { WorkoutComplexId = 1, ExerciseId = 6, OrderNumber = 2, Sets = 3, Repetitions = 10 },
        new() { WorkoutComplexId = 1, ExerciseId = 2, OrderNumber = 3, Sets = 3, Repetitions = 45 },
        new() { WorkoutComplexId = 2, ExerciseId = 5, OrderNumber = 1, Sets = 4, Repetitions = 6 },
        new() { WorkoutComplexId = 2, ExerciseId = 4, OrderNumber = 2, Sets = 4, Repetitions = 8 },
        new() { WorkoutComplexId = 2, ExerciseId = 3, OrderNumber = 3, Sets = 3, Repetitions = 10 },
        new() { WorkoutComplexId = 3, ExerciseId = 4, OrderNumber = 1, Sets = 4, Repetitions = 8 },
        new() { WorkoutComplexId = 3, ExerciseId = 6, OrderNumber = 2, Sets = 3, Repetitions = 12 }
    ];
}
