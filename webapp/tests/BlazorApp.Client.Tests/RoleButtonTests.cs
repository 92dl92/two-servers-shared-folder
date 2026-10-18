using Bunit;
using BlazorApp.Client.Pages;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>Which buttons each role sees on the Register page.</summary>
public sealed class RoleButtonTests : ClientTestContext
{
    /// <inheritdoc />
    protected override void ConfigureServices(BunitServiceProvider services) =>
        services.AddSingleton<IRegistrationService>(new FakeRegistrationService());

    /// <summary>Whether each role may see the registration form and its buttons; written out by hand.</summary>
    public static TheoryData<Role, bool> Table => new()
    {
        { Role.Registrant, true },
        { Role.Reviewer, false },
        { Role.Admin, true },
    };

    [Theory]
    [MemberData(nameof(Table))]
    public void Register_form_and_buttons_follow_the_role(Role role, bool formShown)
    {
        var user = new UserDto("u", "Una", [role]);

        var cut = Render<Register>(p => p.AddCascadingValue(new ActingUser(user)));

        Assert.Equal(formShown, cut.FindAll("form").Count == 1);
        Assert.Equal(formShown, cut.FindAll("button").Any(b => b.TextContent.Trim() == "Next"));
        if (!formShown)
        {
            Assert.Empty(cut.FindAll("button"));
            Assert.Contains("Una (Reviewer) can't create registrations", cut.Find("[role=status]").TextContent);
        }
    }

    [Fact]
    public void Nobody_selected_sees_a_prompt_and_no_buttons()
    {
        var cut = Render<Register>(p => p.AddCascadingValue(new ActingUser(null)));

        Assert.Empty(cut.FindAll("button"));
        Assert.Contains("Acting as", cut.Find("[role=status]").TextContent);
    }
}
