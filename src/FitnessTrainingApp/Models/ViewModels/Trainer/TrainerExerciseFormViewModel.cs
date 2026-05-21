using System.ComponentModel.DataAnnotations;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerExerciseFormViewModel
{
    public int? Id { get; set; }

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

    [Required]
    [StringLength(160)]
    public string Equipment { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string MuscleGroup { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string SafetyNotes { get; set; } = string.Empty;

    [Required]
    [Url]
    [Display(Name = "Media URL")]
    public string MediaUrl { get; set; } = string.Empty;
}
