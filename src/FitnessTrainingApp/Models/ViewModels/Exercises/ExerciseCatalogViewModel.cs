using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Exercises;

public sealed class ExerciseCatalogViewModel
{
    public string? Search { get; set; }
    public DifficultyLevel? Difficulty { get; set; }
    public WorkoutType? WorkoutType { get; set; }
    public string? Equipment { get; set; }
    public string? MuscleGroup { get; set; }
    public IReadOnlyList<ExerciseCardViewModel> Exercises { get; set; } = [];
}
