using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Exercises;

public sealed class ExerciseDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DifficultyLevel Difficulty { get; set; }
    public WorkoutType WorkoutType { get; set; }
    public string Equipment { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string SafetyNotes { get; set; } = string.Empty;
    public string TrainerName { get; set; } = string.Empty;
    public IReadOnlyList<string> MediaUrls { get; set; } = [];
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int CommentCount { get; set; }
    public int? UserRating { get; set; }
    public int? PlaylistItemId { get; set; }
    public string BackUrl { get; set; } = string.Empty;
    public string BackLabel { get; set; } = "Back to catalog";
    public bool HasCustomBackUrl { get; set; }
    public IReadOnlyList<ExerciseCommentViewModel> Comments { get; set; } = [];
}
