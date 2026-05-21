namespace FitnessTrainingApp.Models.Entities;

public sealed class EntityChangeLog : BaseEntity
{
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string ChangesJson { get; set; } = string.Empty;

    public User? User { get; set; }
}
