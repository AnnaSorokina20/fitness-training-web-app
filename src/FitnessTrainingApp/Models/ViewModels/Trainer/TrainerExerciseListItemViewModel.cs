using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerExerciseListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
    public ContentStatus Status { get; set; }
    public string? ModerationComment { get; set; }
    public DateTime UpdatedAt { get; set; }
}
