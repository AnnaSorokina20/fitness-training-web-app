using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Exercises;

namespace FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;

public sealed class WorkoutComplexDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public int DurationMinutes { get; set; }
    public int? PlaylistItemId { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int CommentCount { get; set; }
    public int? UserRating { get; set; }
    public IReadOnlyList<ExerciseCommentViewModel> Comments { get; set; } = [];
    public IReadOnlyList<WorkoutComplexExerciseViewModel> Exercises { get; set; } = [];
}
