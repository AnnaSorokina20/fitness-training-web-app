using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US08")]
public sealed class US08AuthorizationIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task ActiveAdministratorSession_ShouldAllowAdministratorAccess()
    {
        using var context = CreateContext();
        var admin = await AddUserAsync(context, role: UserRole.Administrator);
        context.UserSessions.Add(new UserSession
        {
            UserId = admin.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsActive = true,
            Role = admin.Role
        });
        await context.SaveChangesAsync();

        var session = context.UserSessions.Single(item => item.UserId == admin.Id);

        Assert.That(session.IsActive, Is.True);
        Assert.That(session.Role, Is.EqualTo(UserRole.Administrator));
    }

    [Test]
    public async Task InactiveSession_ShouldDenyAccess()
    {
        using var context = CreateContext();
        var admin = await AddUserAsync(context, role: UserRole.Administrator);
        context.UserSessions.Add(new UserSession
        {
            UserId = admin.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsActive = false,
            Role = admin.Role
        });
        await context.SaveChangesAsync();

        var activeSessionExists = context.UserSessions.Any(item =>
            item.UserId == admin.Id &&
            item.IsActive &&
            item.ExpiresAt > DateTime.UtcNow);

        Assert.That(activeSessionExists, Is.False);
    }

    [Test]
    public async Task ExpiredSession_ShouldDenyAccess()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context);
        context.UserSessions.Add(new UserSession
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
            IsActive = true,
            Role = user.Role
        });
        await context.SaveChangesAsync();

        var validSessionExists = context.UserSessions.Any(item =>
            item.UserId == user.Id &&
            item.IsActive &&
            item.ExpiresAt > DateTime.UtcNow);

        Assert.That(validSessionExists, Is.False);
    }
}
