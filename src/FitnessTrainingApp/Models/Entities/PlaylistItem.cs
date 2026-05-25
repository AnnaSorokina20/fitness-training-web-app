using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.Entities;

public sealed class PlaylistItem : BaseEntity
{
    public int UserId { get; set; }
    public int? ExerciseId { get; set; }
    public int? WorkoutComplexId { get; set; }
    public PlaylistItemType ItemType { get; set; }

    public User? User { get; set; }
    public Exercise? Exercise { get; set; }
    public WorkoutComplex? WorkoutComplex { get; set; }
}
