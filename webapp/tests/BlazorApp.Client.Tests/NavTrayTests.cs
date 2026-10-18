using Bunit;
using BlazorApp.Client.Layout;
using BlazorApp.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>The navigation tray on its own, and inside <see cref="MainLayout"/>.</summary>
public sealed class NavTrayTests : ClientTestContext
{
    /// <inheritdoc />
    /// <remarks>Since phase 2 the layout loads the "Acting as" users.</remarks>
    protected override void ConfigureServices(BunitServiceProvider services) =>
        services.AddSingleton<ICurrentUserService>(new FakeCurrentUserService());

    [Fact]
    public void Collapse_button_reports_the_flipped_state()
    {
        bool? reported = null;
        var cut = Render<NavTray>(p => p
            .Add(x => x.Collapsed, false)
            .Add(x => x.CollapsedChanged, (bool value) => reported = value));

        Assert.Equal("Collapse navigation", cut.Find(".collapse-button").GetAttribute("aria-label"));
        cut.Find(".collapse-button").Click();

        Assert.True(reported);
    }

    [Fact]
    public void Collapsed_tray_has_the_collapsed_class_and_an_expand_label()
    {
        var cut = Render<NavTray>(p => p.Add(x => x.Collapsed, true));

        Assert.Contains("collapsed", cut.Find("nav").ClassList);
        Assert.Equal("Expand navigation", cut.Find(".collapse-button").GetAttribute("aria-label"));
    }

    [Fact]
    public void Open_drawer_shows_a_backdrop_and_every_close_control_raises_OnClose()
    {
        var closes = 0;
        var cut = Render<NavTray>(p => p
            .Add(x => x.MobileOpen, true)
            .Add(x => x.OnClose, () => closes++));

        Assert.Contains("open", cut.Find("nav").ClassList);
        cut.Find(".backdrop").Click();
        cut.Find(".close-button").Click();
        cut.FindAll("a.tray-link")[1].Click();

        Assert.Equal(3, closes);
    }

    [Fact]
    public void Closed_drawer_has_no_backdrop()
    {
        var cut = Render<NavTray>(p => p.Add(x => x.MobileOpen, false));

        Assert.Empty(cut.FindAll(".backdrop"));
        Assert.DoesNotContain("open", cut.Find("nav").ClassList);
    }

    [Fact]
    public void Layout_collapses_the_tray_and_widens_the_content()
    {
        var cut = RenderLayout();

        cut.Find(".collapse-button").Click();

        Assert.Contains("collapsed", cut.Find("nav").ClassList);
        Assert.Contains("tray-collapsed", cut.Find("main").ClassList);

        cut.Find(".collapse-button").Click();

        Assert.DoesNotContain("collapsed", cut.Find("nav").ClassList);
    }

    [Fact]
    public void Layout_drawer_closes_when_a_link_is_followed()
    {
        var cut = RenderLayout();

        Assert.Equal("false", cut.Find(".menu-button").GetAttribute("aria-expanded"));
        cut.Find(".menu-button").Click();
        Assert.Contains("open", cut.Find("nav").ClassList);
        Assert.Equal("true", cut.Find(".menu-button").GetAttribute("aria-expanded"));

        cut.Find("a.tray-link[href=register]").Click();

        Assert.DoesNotContain("open", cut.Find("nav").ClassList);
        Assert.Empty(cut.FindAll(".backdrop"));
        Assert.Equal("false", cut.Find(".menu-button").GetAttribute("aria-expanded"));
    }

    private IRenderedComponent<MainLayout> RenderLayout() =>
        Render<MainLayout>(p => p.Add(x => x.Body, (RenderFragment)(b => b.AddContent(0, "page"))));
}
