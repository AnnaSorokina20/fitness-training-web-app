using FitnessTrainingApp.Models.ViewModels.Shared;

namespace FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;

public sealed class WorkoutComplexCatalogViewModel
{
    public PagedListViewModel<WorkoutComplexCardViewModel> WorkoutComplexes { get; set; } = new();
}
