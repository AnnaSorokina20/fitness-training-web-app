using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class PendingExerciseViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public string SafetyNotes { get; set; } = string.Empty;
    public IReadOnlyList<string> MediaUrls { get; set; } = [];
    public DateTime SubmittedAt { get; set; }
}
