using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.Trainer;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTrainingApp.Areas.Trainer.Controllers;

[Area("Trainer")]
[Authorize(Roles = "Trainer")]
public sealed class WorkoutComplexesController(IWorkoutComplexService workoutComplexService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var trainerId = User.GetUserId();
        var complexes = await workoutComplexService.GetForTrainerAsync(trainerId);

        return View(new TrainerWorkoutComplexIndexViewModel
        {
            WorkoutComplexes = complexes.Select(complex => new TrainerWorkoutComplexListItemViewModel
            {
                Id = complex.Id,
                Name = complex.Name,
                Difficulty = complex.Difficulty,
                WorkoutType = complex.WorkoutType,
                DurationMinutes = complex.DurationMinutes,
                ExerciseCount = complex.WorkoutComplexExercises.Count,
                Status = complex.Status,
                UpdatedAt = complex.UpdatedAt ?? complex.CreatedAt
            }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await PopulateExerciseOptionsAsync(new TrainerWorkoutComplexFormViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerWorkoutComplexFormViewModel model)
    {
        var selectedExercises = model.Exercises
            .Where(item => item.ExerciseId > 0)
            .ToList();

        if (selectedExercises.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Select at least one exercise.");
        }

        if (!ModelState.IsValid)
        {
            return View(await PopulateExerciseOptionsAsync(model));
        }

        var created = await workoutComplexService.CreateForTrainerAsync(
            ToEntity(model, User.GetUserId()),
            selectedExercises.Select((item, index) => new WorkoutComplexExercise
            {
                ExerciseId = item.ExerciseId,
                OrderNumber = index + 1,
                Sets = item.Sets,
                Repetitions = item.Repetitions
            }).ToList());

        if (!created)
        {
            ModelState.AddModelError(string.Empty, "Workout complex was not created. Check the selected exercises.");
            return View(await PopulateExerciseOptionsAsync(model));
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<TrainerWorkoutComplexFormViewModel> PopulateExerciseOptionsAsync(TrainerWorkoutComplexFormViewModel model)
    {
        var exercises = await workoutComplexService.GetAvailableExercisesAsync();
        model.ExerciseOptions = exercises
            .Select(exercise => new SelectListItem($"{exercise.Name} ({exercise.MuscleGroup})", exercise.Id.ToString()))
            .ToList();

        return model;
    }

    private static WorkoutComplex ToEntity(TrainerWorkoutComplexFormViewModel model, int trainerId)
    {
        return new WorkoutComplex
        {
            Name = model.Name,
            Description = model.Description,
            Difficulty = model.Difficulty,
            WorkoutType = model.WorkoutType,
            DurationMinutes = model.DurationMinutes,
            TrainerId = trainerId
        };
    }
}
