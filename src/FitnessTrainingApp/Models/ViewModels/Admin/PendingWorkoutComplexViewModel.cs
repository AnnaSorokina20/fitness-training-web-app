using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;

namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class PendingWorkoutComplexViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public int ExerciseCount { get; set; }
    public IReadOnlyList<WorkoutComplexExerciseViewModel> Exercises { get; set; } = [];
    public DateTime SubmittedAt { get; set; }
}
