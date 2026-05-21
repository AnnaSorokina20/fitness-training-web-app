using FitnessTrainingApp.Models.ViewModels.Exercises;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;

namespace FitnessTrainingApp.Models.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public int ExerciseCount { get; set; }
    public int WorkoutComplexCount { get; set; }
    public int HomeWorkoutCount { get; set; }
    public int GymWorkoutCount { get; set; }
    public IReadOnlyList<ExerciseCardViewModel> FeaturedExercises { get; set; } = [];
    public IReadOnlyList<WorkoutComplexCardViewModel> FeaturedComplexes { get; set; } = [];
}
