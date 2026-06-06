using System.ComponentModel.DataAnnotations;

namespace FitnessTrainingApp.Models.ViewModels.Account;

public sealed class ProfileViewModel
{
    [Required]
    [StringLength(160, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
