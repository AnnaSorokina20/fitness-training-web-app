namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerExerciseIndexViewModel
{
    public IReadOnlyList<TrainerExerciseListItemViewModel> Exercises { get; set; } = [];
}
