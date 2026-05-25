using FitnessTrainingApp.Models.ViewModels.Admin;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public sealed class AdminLogsController(IAdminLogService adminLogService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var logs = await adminLogService.GetAllAsync();

        return View(new AdminLogIndexViewModel
        {
            Logs = logs.Select(log => new AdminLogItemViewModel
            {
                Id = log.Id,
                AdminName = log.Admin?.FullName ?? "Administrator",
                Action = log.Action,
                EntityName = log.EntityName,
                EntityId = log.EntityId,
                TargetUserId = log.TargetUserId,
                CreatedAt = log.CreatedAt
            }).ToList()
        });
    }
}
