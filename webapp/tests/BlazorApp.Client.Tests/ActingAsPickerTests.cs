using Bunit;
using Bunit.TestDoubles;
using BlazorApp.Client.Layout;
using BlazorApp.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>The "Acting as" picker in the layout.</summary>
public sealed class ActingAsPickerTests : ClientTestContext
{
    private readonly FakeCurrentUserService _users = new() { Current = FakeCurrentUserService.Rita };

    /// <inheritdoc />
    protected override void ConfigureServices(BunitServiceProvider services) =>
        services.AddSingleton<ICurrentUserService>(_users);

    [Fact]
    public void Lists_every_user_with_roles_and_selects_the_current_one()
    {
        var cut = RenderLayout();

        var options = cut.FindAll(".acting-as option").Select(o => o.TextContent.Trim()).ToList();
        Assert.Equal(["— nobody —", "Alice (Registrant)", "Rita (Reviewer)", "Ada (Admin)"], options);
        Assert.Equal("rita", cut.Find(".acting-as select").GetAttribute("value"));
    }

    [Fact]
    public void Choosing_a_user_saves_it_and_reloads_the_page()
    {
        var cut = RenderLayout();
        var navigation = Services.GetRequiredService<BunitNavigationManager>();

        cut.Find(".acting-as select").Change("alice");

        Assert.Equal(["alice"], _users.Changes);
        var reload = Assert.Single(navigation.History);
        Assert.True(reload.Options.ForceLoad);
    }

    [Fact]
    public void Choosing_nobody_clears_the_user()
    {
        var cut = RenderLayout();

        cut.Find(".acting-as select").Change("");

        Assert.Equal([null], _users.Changes);
    }

    [Fact]
    public void Pages_receive_the_acting_user()
    {
        var cut = Render<MainLayout>(p => p.Add(
            x => x.Body,
            (RenderFragment)(b =>
            {
                b.OpenComponent<ActingUserProbe>(0);
                b.CloseComponent();
            })));

        Assert.Equal("rita", cut.FindComponent<ActingUserProbe>().Instance.Acting?.User?.Id);
    }

    private IRenderedComponent<MainLayout> RenderLayout() =>
        Render<MainLayout>(p => p.Add(x => x.Body, (RenderFragment)(b => b.AddContent(0, "page"))));

    /// <summary>A page stand-in that records the cascaded <see cref="ActingUser"/>.</summary>
    private sealed class ActingUserProbe : ComponentBase
    {
        /// <summary>The cascaded value.</summary>
        [CascadingParameter] public ActingUser? Acting { get; set; }
    }
}
