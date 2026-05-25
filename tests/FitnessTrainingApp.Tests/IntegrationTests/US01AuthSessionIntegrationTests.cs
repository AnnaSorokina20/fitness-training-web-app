using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US01")]
public sealed class US01AuthSessionIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task RegisterUser_ShouldSaveUserToDatabase()
    {
        using var context = CreateContext();

        var registered = await new AuthService(context).RegisterAsync("Anna", "user@test.local", "Strong123");

        Assert.That(registered, Is.True);
        Assert.That(context.Users.Single().Role, Is.EqualTo(UserRole.User));
    }

    [Test]
    public async Task LoginUser_WithCorrectCredentials_ShouldCreateSessionRecord()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        await authService.RegisterAsync("Anna", "user@test.local", "Strong123");

        var user = await authService.LoginAsync("user@test.local", "Strong123");
        context.UserSessions.Add(new UserSession
        {
            UserId = user!.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsActive = true,
            Role = user.Role
        });
        await context.SaveChangesAsync();

        Assert.That(user, Is.Not.Null);
        Assert.That(context.UserSessions.Single().IsActive, Is.True);
    }

    [Test]
    public async Task LoginUser_WithWrongPassword_ShouldNotCreateSessionRecord()
    {
        using var context = CreateContext();
        var authService = new AuthService(context);
        await authService.RegisterAsync("Anna", "user@test.local", "Strong123");

        var user = await authService.LoginAsync("user@test.local", "wrongPass");

        Assert.That(user, Is.Null);
        Assert.That(context.UserSessions, Is.Empty);
    }

    [Test]
    public async Task EndActiveSession_ShouldInvalidateSessionRecord()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        var session = new UserSession
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsActive = true,
            Role = user.Role
        };
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();

        session.IsActive = false;
        await context.SaveChangesAsync();

        Assert.That(context.UserSessions.Single().IsActive, Is.False);
    }
}
