using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Admin;

public sealed class UserManagementIndexViewModel
{
    public int CurrentUserId { get; set; }
    public IReadOnlyList<UserListItemViewModel> Users { get; set; } = [];
    public IReadOnlyList<UserRole> Roles { get; set; } =
    [
        UserRole.User,
        UserRole.Trainer,
        UserRole.Administrator
    ];
}
