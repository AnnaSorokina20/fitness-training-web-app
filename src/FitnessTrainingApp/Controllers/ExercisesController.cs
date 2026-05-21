using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Exercises;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class ExercisesController(IExerciseService exerciseService) : Controller
{
    public async Task<IActionResult> Index(
        string? search,
        DifficultyLevel? difficulty,
        WorkoutType? workoutType,
        string? equipment,
        string? muscleGroup)
    {
        var exercises = string.IsNullOrWhiteSpace(search)
            ? await exerciseService.FilterAsync(difficulty, equipment, muscleGroup, workoutType)
            : await exerciseService.SearchAsync(search);

        if (!string.IsNullOrWhiteSpace(search))
        {
            exercises = ApplyFilters(exercises, difficulty, workoutType, equipment, muscleGroup);
        }

        var viewModel = new ExerciseCatalogViewModel
        {
            Search = search,
            Difficulty = difficulty,
            WorkoutType = workoutType,
            Equipment = equipment,
            MuscleGroup = muscleGroup,
            Exercises = exercises.Select(ToCardViewModel).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var exercise = await exerciseService.GetDetailsAsync(id);

        if (exercise is null)
        {
            return NotFound();
        }

        return View(new ExerciseDetailsViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Difficulty = exercise.Difficulty,
            WorkoutType = exercise.WorkoutType,
            Equipment = exercise.Equipment,
            MuscleGroup = exercise.MuscleGroup,
            SafetyNotes = exercise.SafetyNotes,
            MediaUrls = exercise.MediaFiles.Select(file => file.Url).ToList(),
            AverageRating = exercise.Ratings.Count == 0 ? 0 : exercise.Ratings.Average(rating => rating.Value),
            CommentCount = exercise.Comments.Count
        });
    }

    private static IReadOnlyList<Exercise> ApplyFilters(
        IReadOnlyList<Exercise> exercises,
        DifficultyLevel? difficulty,
        WorkoutType? workoutType,
        string? equipment,
        string? muscleGroup)
    {
        var query = exercises.AsEnumerable();

        if (difficulty.HasValue)
        {
            query = query.Where(exercise => exercise.Difficulty == difficulty.Value);
        }

        if (workoutType.HasValue)
        {
            query = query.Where(exercise => exercise.WorkoutType == workoutType.Value);
        }

        if (!string.IsNullOrWhiteSpace(equipment))
        {
            query = query.Where(exercise => exercise.Equipment.Contains(equipment, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            query = query.Where(exercise => exercise.MuscleGroup.Contains(muscleGroup, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }

    private static ExerciseCardViewModel ToCardViewModel(Exercise exercise)
    {
        return new ExerciseCardViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Difficulty = exercise.Difficulty,
            WorkoutType = exercise.WorkoutType,
            Equipment = exercise.Equipment,
            MuscleGroup = exercise.MuscleGroup
        };
    }
}
