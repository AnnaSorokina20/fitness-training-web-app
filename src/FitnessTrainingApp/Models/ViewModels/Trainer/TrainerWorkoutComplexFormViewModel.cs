using System.ComponentModel.DataAnnotations;
using FitnessTrainingApp.Models.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerWorkoutComplexFormViewModel
{
    [Required]
    [StringLength(160, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DifficultyLevel Difficulty { get; set; }

    [Required]
    public WorkoutType WorkoutType { get; set; }

    [Range(1, 240)]
    [Display(Name = "Duration, minutes")]
    public int DurationMinutes { get; set; } = 30;

    public List<TrainerWorkoutComplexExerciseFormViewModel> Exercises { get; set; } =
    [
        new()
    ];

    public IReadOnlyList<SelectListItem> ExerciseOptions { get; set; } = [];
}
