using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.Trainer;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Areas.Trainer.Controllers;

[Area("Trainer")]
[Authorize(Roles = "Trainer")]
public sealed class ExercisesController(
    IExerciseService exerciseService,
    IContentDeletionService contentDeletionService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var trainerId = User.GetUserId();
        var exercises = await exerciseService.GetForTrainerAsync(trainerId);

        return View(new TrainerExerciseIndexViewModel
        {
            Exercises = exercises.Select(exercise => new TrainerExerciseListItemViewModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Difficulty = exercise.Difficulty,
                WorkoutType = exercise.WorkoutType,
                MuscleGroup = exercise.MuscleGroup,
                Status = exercise.Status,
                ModerationComment = exercise.ModerationComment,
                UpdatedAt = exercise.UpdatedAt ?? exercise.CreatedAt
            }).ToList()
        });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainerExerciseFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerExerciseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var created = await exerciseService.CreateForTrainerAsync(
            ToEntity(model, User.GetUserId()),
            model.MediaUrls,
            model.UploadedImages);

        if (!created)
        {
            ModelState.AddModelError(string.Empty, "Exercise was not created. Check the form data.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var exercise = await exerciseService.GetTrainerExerciseAsync(id, User.GetUserId());

        if (exercise is null)
        {
            return NotFound();
        }

        return View(ToFormViewModel(exercise));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await contentDeletionService.DeleteExerciseAsync(id, User.GetUserId(), false);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TrainerExerciseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View(model);
        }

        var updated = await exerciseService.UpdateForTrainerAsync(
            id,
            User.GetUserId(),
            ToEntity(model, User.GetUserId()),
            model.MediaUrls,
            model.UploadedImages);

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, "Exercise was not updated. Check the form data.");
            model.Id = id;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    private static Exercise ToEntity(TrainerExerciseFormViewModel model, int trainerId)
    {
        return new Exercise
        {
            Name = model.Name,
            Description = model.Description,
            Difficulty = model.Difficulty,
            WorkoutType = model.WorkoutType,
            Equipment = model.Equipment,
            MuscleGroup = model.MuscleGroup,
            SafetyNotes = model.SafetyNotes,
            TrainerId = trainerId
        };
    }

    private static TrainerExerciseFormViewModel ToFormViewModel(Exercise exercise)
    {
        return new TrainerExerciseFormViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            Difficulty = exercise.Difficulty,
            WorkoutType = exercise.WorkoutType,
            Equipment = exercise.Equipment,
            MuscleGroup = exercise.MuscleGroup,
            SafetyNotes = exercise.SafetyNotes,
            MediaUrls = string.Join(Environment.NewLine, exercise.MediaFiles.Select(file => file.Url))
        };
    }
}
