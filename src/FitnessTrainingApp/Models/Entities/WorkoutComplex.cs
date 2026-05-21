using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.Entities;

public sealed class WorkoutComplex : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.PendingModeration;
    public int TrainerId { get; set; }

    public User? Trainer { get; set; }
    public ICollection<WorkoutComplexExercise> WorkoutComplexExercises { get; set; } = [];
}
