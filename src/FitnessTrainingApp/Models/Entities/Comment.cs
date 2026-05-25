namespace FitnessTrainingApp.Models.Entities;

public sealed class Comment : BaseEntity
{
    public int UserId { get; set; }
    public int? ExerciseId { get; set; }
    public int? WorkoutComplexId { get; set; }
    public string Text { get; set; } = string.Empty;

    public User? User { get; set; }
    public Exercise? Exercise { get; set; }
    public WorkoutComplex? WorkoutComplex { get; set; }
}
