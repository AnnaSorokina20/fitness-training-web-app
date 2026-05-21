namespace FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;

public sealed class WorkoutComplexExerciseViewModel
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string Equipment { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}
