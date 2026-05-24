using System.ComponentModel.DataAnnotations;
using FitnessTrainingApp.Models.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerExerciseFormViewModel : IValidatableObject
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

    [Display(Name = "Media URLs")]
    public string? MediaUrls { get; set; }

    [Display(Name = "Upload images")]
    public IReadOnlyList<IFormFile>? UploadedImages { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasMediaUrls = !string.IsNullOrWhiteSpace(MediaUrls);
        var hasUploadedImages = UploadedImages?.Any(file => file.Length > 0) == true;

        if (!hasMediaUrls && !hasUploadedImages)
        {
            yield return new ValidationResult(
                "Add at least one media URL or upload an image.",
                [nameof(MediaUrls), nameof(UploadedImages)]);
        }
    }
}
