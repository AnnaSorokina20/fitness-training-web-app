namespace FitnessTrainingApp.Models.Entities;

public sealed class AdminLog : BaseEntity
{
    public int AdminId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public int? TargetUserId { get; set; }

    public User? Admin { get; set; }
}
