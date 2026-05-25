using System.Diagnostics;
using FitnessTrainingApp.Models;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Exercises;
using FitnessTrainingApp.Models.ViewModels.Home;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class HomeController(
    IExerciseService exerciseService,
    IWorkoutComplexService workoutComplexService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var exercises = await exerciseService.GetAllAsync();
        var complexes = await workoutComplexService.GetAllAsync();

        return View(new HomeIndexViewModel
        {
            ExerciseCount = exercises.Count,
            WorkoutComplexCount = complexes.Count,
            HomeWorkoutCount = exercises.Count(exercise => exercise.WorkoutType == WorkoutType.Home),
            GymWorkoutCount = exercises.Count(exercise => exercise.WorkoutType == WorkoutType.Gym),
            FeaturedExercises = exercises.Take(3).Select(ToExerciseCardViewModel).ToList(),
            FeaturedComplexes = complexes.Take(2).Select(ToWorkoutComplexCardViewModel).ToList()
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static ExerciseCardViewModel ToExerciseCardViewModel(Exercise exercise)
    {
        return new ExerciseCardViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Difficulty = exercise.Difficulty,
            WorkoutType = exercise.WorkoutType,
            Equipment = exercise.Equipment,
            MuscleGroup = exercise.MuscleGroup,
            ThumbnailUrl = GetThumbnailUrl(exercise)
        };
    }

    private static string? GetThumbnailUrl(Exercise exercise)
    {
        return exercise.MediaFiles
            .FirstOrDefault(file => IsImageMedia(file))
            ?.Url;
    }

    private static bool IsImageMedia(MediaFile mediaFile)
    {
        return mediaFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
               mediaFile.Url.StartsWith("/uploads/exercises/", StringComparison.OrdinalIgnoreCase);
    }

    private static WorkoutComplexCardViewModel ToWorkoutComplexCardViewModel(WorkoutComplex complex)
    {
        return new WorkoutComplexCardViewModel
        {
            Id = complex.Id,
            Name = complex.Name,
            Description = complex.Description,
            Difficulty = complex.Difficulty,
            WorkoutType = complex.WorkoutType,
            DurationMinutes = complex.DurationMinutes,
            ExerciseCount = complex.WorkoutComplexExercises.Count
        };
    }
}
