namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class AdminLogIndexViewModel
{
    public IReadOnlyList<AdminLogItemViewModel> Logs { get; set; } = [];
}
