using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Models.ViewModels.Playlist;

public sealed class PlaylistItemViewModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public PlaylistItemType ItemType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Meta { get; set; } = string.Empty;
}
