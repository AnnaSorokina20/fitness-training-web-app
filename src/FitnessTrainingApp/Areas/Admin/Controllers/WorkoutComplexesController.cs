using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.Trainer;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessTrainingApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public sealed class WorkoutComplexesController(IWorkoutComplexService workoutComplexService) : Controller
{
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

        if (selectedExercises.Select(item => item.ExerciseId).Distinct().Count() != selectedExercises.Count)
        {
            ModelState.AddModelError(string.Empty, "Select each exercise only once.");
        }

        if (!ModelState.IsValid)
        {
            return View(await PopulateExerciseOptionsAsync(model));
        }

        var created = await workoutComplexService.CreatePublishedAsync(
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

        return RedirectToAction("Index", "Moderation", new { area = "Admin" });
    }

    private async Task<TrainerWorkoutComplexFormViewModel> PopulateExerciseOptionsAsync(TrainerWorkoutComplexFormViewModel model)
    {
        var exercises = await workoutComplexService.GetAvailableExercisesAsync();
        model.ExerciseOptions = exercises
            .Select(exercise => new SelectListItem($"{exercise.Name} ({exercise.MuscleGroup})", exercise.Id.ToString()))
            .ToList();

        return model;
    }

    private static WorkoutComplex ToEntity(TrainerWorkoutComplexFormViewModel model, int adminId)
    {
        return new WorkoutComplex
        {
            Name = model.Name,
            Description = model.Description,
            Difficulty = model.Difficulty,
            WorkoutType = model.WorkoutType,
            DurationMinutes = model.DurationMinutes,
            TrainerId = adminId
        };
    }
}
