using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Admin;
using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public sealed class UsersController(IUserManagementService userManagementService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManagementService.GetAllAsync();

        return View(new UserManagementIndexViewModel
        {
            CurrentUserId = User.GetUserId(),
            Users = users.Select(user => new UserListItemViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int id, UserRole role)
    {
        await userManagementService.ChangeRoleAsync(id, role, User.GetUserId());
        return RedirectToAction(nameof(Index));
    }
}
