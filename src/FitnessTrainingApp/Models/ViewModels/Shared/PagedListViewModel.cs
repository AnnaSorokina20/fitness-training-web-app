namespace FitnessTrainingApp.Models.ViewModels.Shared;

public sealed class PagedListViewModel<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public PaginationViewModel Pagination { get; set; } = new();
}
