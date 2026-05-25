using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.Entities;

public sealed class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string SafetyNotes { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.PendingModeration;
    public string? ModerationComment { get; set; }
    public int TrainerId { get; set; }

    public User? Trainer { get; set; }
    public ICollection<MediaFile> MediaFiles { get; set; } = [];
    public ICollection<WorkoutComplexExercise> WorkoutComplexExercises { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
}
