namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class ModerationIndexViewModel
{
    public IReadOnlyList<PendingExerciseViewModel> PendingExercises { get; set; } = [];
}
