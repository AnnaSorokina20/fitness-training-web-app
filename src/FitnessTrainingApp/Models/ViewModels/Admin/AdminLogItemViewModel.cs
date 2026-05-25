namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class AdminLogItemViewModel
{
    public int Id { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public int? TargetUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
