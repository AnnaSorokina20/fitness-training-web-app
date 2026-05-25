namespace FitnessTrainingApp.Models.Entities;

public sealed class Rating : BaseEntity
{
    public int UserId { get; set; }
    public int? ExerciseId { get; set; }
    public int? WorkoutComplexId { get; set; }
    public int Value { get; set; }

    public User? User { get; set; }
    public Exercise? Exercise { get; set; }
    public WorkoutComplex? WorkoutComplex { get; set; }
}
