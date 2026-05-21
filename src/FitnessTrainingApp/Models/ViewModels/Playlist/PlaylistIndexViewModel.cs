namespace FitnessTrainingApp.Models.ViewModels.Playlist;

public sealed class PlaylistIndexViewModel
{
    public IReadOnlyList<PlaylistItemViewModel> Items { get; set; } = [];
}
