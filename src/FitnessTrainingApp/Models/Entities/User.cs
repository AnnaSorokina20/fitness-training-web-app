using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.Entities;

public sealed class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<Exercise> Exercises { get; set; } = [];
    public ICollection<WorkoutComplex> WorkoutComplexes { get; set; } = [];
    public ICollection<PlaylistItem> PlaylistItems { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
    public ICollection<UserSession> Sessions { get; set; } = [];
}
