using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Exercises;
using FitnessTrainingApp.Models.ViewModels.Shared;
using FitnessTrainingApp.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class ExercisesController(
    IExerciseService exerciseService,
    ICommentService commentService,
    IRatingService ratingService,
    IPlaylistService playlistService) : Controller
{
    public async Task<IActionResult> Index(
        string? search,
        DifficultyLevel? difficulty,
        WorkoutType? workoutType,
        string? equipment,
        string? muscleGroup,
        int page = 1,
        int pageSize = 12)
    {
        var exercises = string.IsNullOrWhiteSpace(search)
            ? await exerciseService.FilterAsync(difficulty, equipment, muscleGroup, workoutType)
            : await exerciseService.SearchAsync(search);

        if (!string.IsNullOrWhiteSpace(search))
        {
            exercises = ApplyFilters(exercises, difficulty, workoutType, equipment, muscleGroup);
        }

        var cards = exercises.Select(ToCardViewModel).ToList();
        var pagedExercises = CreatePagedList(
            cards,
            page,
            pageSize,
            "Exercises",
            new Dictionary<string, string>
            {
                ["search"] = search ?? string.Empty,
                ["difficulty"] = difficulty?.ToString() ?? string.Empty,
                ["workoutType"] = workoutType?.ToString() ?? string.Empty,
                ["equipment"] = equipment ?? string.Empty,
                ["muscleGroup"] = muscleGroup ?? string.Empty
            });

        var viewModel = new ExerciseCatalogViewModel
        {
            Search = search,
            Difficulty = difficulty,
            WorkoutType = workoutType,
            Equipment = equipment,
            MuscleGroup = muscleGroup,
            Exercises = pagedExercises
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

        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : 0;
        var comments = await commentService.GetForExerciseAsync(id);

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
            TrainerName = exercise.Trainer?.FullName ?? "Trainer",
            MediaUrls = exercise.MediaFiles.Select(file => file.Url).ToList(),
            AverageRating = await ratingService.CalculateAverageAsync(id),
            RatingCount = await ratingService.CountAsync(id),
            CommentCount = comments.Count,
            UserRating = userId == 0 ? null : await ratingService.GetUserRatingAsync(userId, id),
            PlaylistItemId = userId == 0 ? null : await playlistService.GetExercisePlaylistItemIdAsync(userId, id),
            Comments = comments.Select(comment => new ExerciseCommentViewModel
            {
                AuthorName = comment.User?.FullName ?? "User",
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            }).ToList()
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int exerciseId, string text)
    {
        await commentService.AddAsync(User.GetUserId(), exerciseId, text);
        return RedirectToAction(nameof(Details), new { id = exerciseId });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rate(int exerciseId, int value)
    {
        await ratingService.AddOrUpdateAsync(User.GetUserId(), exerciseId, value);
        return RedirectToAction(nameof(Details), new { id = exerciseId });
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

    private static PagedListViewModel<T> CreatePagedList<T>(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        string controllerName,
        IDictionary<string, string> routeValues)
    {
        var normalizedPageSize = NormalizePageSize(pageSize);
        var totalPages = Math.Max(1, (int)Math.Ceiling(items.Count / (double)normalizedPageSize));
        var normalizedPage = Math.Clamp(page, 1, totalPages);

        return new PagedListViewModel<T>
        {
            Items = items
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = normalizedPage,
                PageSize = normalizedPageSize,
                TotalItems = items.Count,
                ControllerName = controllerName,
                RouteValues = routeValues
                    .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                    .ToDictionary(item => item.Key, item => item.Value)
            }
        };
    }

    private static int NormalizePageSize(int pageSize)
    {
        return pageSize is 24 or 48 ? pageSize : 12;
    }
}
