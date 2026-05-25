using System.ComponentModel.DataAnnotations;

namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerWorkoutComplexExerciseFormViewModel
{
    [Display(Name = "Exercise")]
    public int ExerciseId { get; set; }

    [Range(1, 20)]
    public int Sets { get; set; } = 3;

    [Range(1, 100)]
    public int Repetitions { get; set; } = 10;
}
