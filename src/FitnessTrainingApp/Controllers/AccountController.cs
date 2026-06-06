using System.Security.Claims;
using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.ViewModels.Account;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class AccountController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await authService.LoginAsync(model.Email, model.Password);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(user.Id, user.FullName, user.Email, user.Role.ToString());

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var registered = await authService.RegisterAsync(model.FullName, model.Email, model.Password);

        if (!registered)
        {
            ModelState.AddModelError(string.Empty, "Registration failed. Check the form data or use another email.");
            return View(model);
        }

        var user = await authService.LoginAsync(model.Email, model.Password);

        if (user is not null)
        {
            await SignInAsync(user.Id, user.FullName, user.Email, user.Role.ToString());
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var model = await BuildProfileViewModelAsync();
        if (model is null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        ModelState.Remove(nameof(ProfileViewModel.CurrentPassword));
        ModelState.Remove(nameof(ProfileViewModel.NewPassword));
        ModelState.Remove(nameof(ProfileViewModel.ConfirmNewPassword));

        if (!ModelState.IsValid)
        {
            model.Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            return View("Profile", model);
        }

        var updated = await authService.UpdateProfileAsync(User.GetUserId(), model.FullName, model.Email);

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, "Profile update failed. Check the form data or use another email.");
            model.Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            return View("Profile", model);
        }

        var user = await authService.GetUserAsync(User.GetUserId());
        if (user is not null)
        {
            await SignInAsync(user.Id, user.FullName, user.Email, user.Role.ToString());
        }

        TempData["ProfileStatus"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        ModelState.Remove(nameof(ProfileViewModel.FullName));
        ModelState.Remove(nameof(ProfileViewModel.Email));
        ModelState.Remove(nameof(ProfileViewModel.Role));

        if (string.IsNullOrWhiteSpace(model.CurrentPassword))
        {
            ModelState.AddModelError(nameof(ProfileViewModel.CurrentPassword), "Current password is required.");
        }

        if (string.IsNullOrWhiteSpace(model.NewPassword) || model.NewPassword.Length < 8 || !model.NewPassword.Any(char.IsDigit))
        {
            ModelState.AddModelError(nameof(ProfileViewModel.NewPassword), "New password must contain at least 8 characters and one digit.");
        }

        if (model.NewPassword != model.ConfirmNewPassword)
        {
            ModelState.AddModelError(nameof(ProfileViewModel.ConfirmNewPassword), "Password confirmation does not match.");
        }

        if (!ModelState.IsValid)
        {
            var profile = await BuildProfileViewModelAsync();
            if (profile is null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }

            profile.CurrentPassword = model.CurrentPassword;
            profile.NewPassword = model.NewPassword;
            profile.ConfirmNewPassword = model.ConfirmNewPassword;
            return View("Profile", profile);
        }

        var changed = await authService.ChangePasswordAsync(User.GetUserId(), model.CurrentPassword, model.NewPassword);

        if (!changed)
        {
            ModelState.AddModelError(nameof(ProfileViewModel.CurrentPassword), "Current password is incorrect.");
            var profile = await BuildProfileViewModelAsync();
            if (profile is null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }

            return View("Profile", profile);
        }

        TempData["ProfileStatus"] = "Password changed.";
        return RedirectToAction(nameof(Profile));
    }

    private async Task<ProfileViewModel?> BuildProfileViewModelAsync()
    {
        var user = await authService.GetUserAsync(User.GetUserId());
        if (user is null)
        {
            return null;
        }

        return new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private async Task SignInAsync(int userId, string fullName, string email, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}
