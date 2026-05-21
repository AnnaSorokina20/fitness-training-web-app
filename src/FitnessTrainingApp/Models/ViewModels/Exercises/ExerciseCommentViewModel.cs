namespace FitnessTrainingApp.Models.ViewModels.Exercises;

public sealed class ExerciseCommentViewModel
{
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
