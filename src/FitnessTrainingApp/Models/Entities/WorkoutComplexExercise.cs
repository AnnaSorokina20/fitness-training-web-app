namespace FitnessTrainingApp.Models.Entities;

public sealed class WorkoutComplexExercise
{
    public int WorkoutComplexId { get; set; }
    public int ExerciseId { get; set; }
    public int OrderNumber { get; set; }
    public int Sets { get; set; }
    public int Repetitions { get; set; }

    public WorkoutComplex? WorkoutComplex { get; set; }
    public Exercise? Exercise { get; set; }
}
