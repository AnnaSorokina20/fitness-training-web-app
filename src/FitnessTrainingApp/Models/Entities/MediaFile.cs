namespace FitnessTrainingApp.Models.Entities;

public sealed class MediaFile : BaseEntity
{
    public int ExerciseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public Exercise? Exercise { get; set; }
}
