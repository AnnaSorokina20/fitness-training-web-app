using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class WorkoutComplexesController(IWorkoutComplexService workoutComplexService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var complexes = await workoutComplexService.GetAllAsync();

        return View(complexes.Select(ToCardViewModel).ToList());
    }

    public async Task<IActionResult> Details(int id)
    {
        var complex = await workoutComplexService.GetDetailsAsync(id);

        if (complex is null)
        {
            return NotFound();
        }

        return View(new WorkoutComplexDetailsViewModel
        {
            Id = complex.Id,
            Name = complex.Name,
            Description = complex.Description,
            Difficulty = complex.Difficulty,
            WorkoutType = complex.WorkoutType,
            DurationMinutes = complex.DurationMinutes,
            Exercises = complex.WorkoutComplexExercises
                .OrderBy(item => item.OrderNumber)
                .Select(item => new WorkoutComplexExerciseViewModel
                {
                    ExerciseId = item.ExerciseId,
                    ExerciseName = item.Exercise?.Name ?? "Exercise",
                    MuscleGroup = item.Exercise?.MuscleGroup ?? string.Empty,
                    Equipment = item.Exercise?.Equipment ?? string.Empty,
                    OrderNumber = item.OrderNumber,
                    Sets = item.Sets,
                    Repetitions = item.Repetitions
                })
                .ToList()
        });
    }

    private static WorkoutComplexCardViewModel ToCardViewModel(WorkoutComplex complex)
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
