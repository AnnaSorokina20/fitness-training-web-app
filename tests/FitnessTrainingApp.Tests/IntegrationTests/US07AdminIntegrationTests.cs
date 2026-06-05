using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;

namespace FitnessTrainingApp.Tests.IntegrationTests;

[TestFixture]
[Category("Integration")]
[Category("US07")]
public sealed class US07AdminIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GetAllUsers_ByAdministratorWorkflow_ShouldReturnUsers()
    {
        using var context = CreateContext();
        await AddUserAsync(context, 1, UserRole.Administrator);
        await AddUserAsync(context, 2, UserRole.User);

        var users = await new UserManagementService(context).GetAllAsync();

        Assert.That(users, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task ChangeUserRole_ByAdministrator_ShouldUpdateUserRole()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        var changed = await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, admin.Id);

        Assert.That(changed, Is.True);
        Assert.That(context.Users.Find(user.Id)?.Role, Is.EqualTo(UserRole.Trainer));
    }

    [Test]
    public async Task ChangeUserRole_ShouldCreateAdminLogRecord()
    {
        using var context = CreateContext();
        var user = await AddUserAsync(context, 1, UserRole.User);
        var admin = await AddUserAsync(context, 2, UserRole.Administrator);

        await new UserManagementService(context).ChangeRoleAsync(user.Id, UserRole.Trainer, admin.Id);
        var logs = await new AdminLogService(context).GetAllAsync();

        Assert.That(logs.Single().TargetUserId, Is.EqualTo(user.Id));
        Assert.That(logs.Single().AdminId, Is.EqualTo(admin.Id));
    }

    [Test]
    public async Task ChangeOwnRole_ShouldBeDenied()
    {
        using var context = CreateContext();
        var admin = await AddUserAsync(context, 1, UserRole.Administrator);

        var changed = await new UserManagementService(context).ChangeRoleAsync(admin.Id, UserRole.User, admin.Id);

        Assert.That(changed, Is.False);
    }
}
