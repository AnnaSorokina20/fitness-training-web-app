using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.Entities;

public sealed class UserSession : BaseEntity
{
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public UserRole Role { get; set; }

    public User? User { get; set; }
}
