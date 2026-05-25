using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class UserListItemViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
}
