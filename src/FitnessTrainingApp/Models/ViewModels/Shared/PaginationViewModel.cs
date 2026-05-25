namespace FitnessTrainingApp.Models.ViewModels.Shared;

public sealed class PaginationViewModel
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalItems { get; set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
    public IReadOnlyList<int> PageSizeOptions { get; set; } = [12, 24, 48];
    public string ActionName { get; set; } = "Index";
    public string ControllerName { get; set; } = string.Empty;
    public IDictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
