using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerWorkoutComplexListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public int ExerciseCount { get; set; }
    public ContentStatus Status { get; set; }
    public string? ModerationComment { get; set; }
    public DateTime UpdatedAt { get; set; }
}
