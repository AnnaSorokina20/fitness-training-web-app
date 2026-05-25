namespace FitnessTrainingApp.Models.ViewModels.Trainer;

public sealed class TrainerWorkoutComplexIndexViewModel
{
    public IReadOnlyList<TrainerWorkoutComplexListItemViewModel> WorkoutComplexes { get; set; } = [];
}
