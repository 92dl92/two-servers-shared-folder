using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client;

/// <summary>
/// The "Acting as" user, cascaded from <see cref="Layout.MainLayout"/> to every page.
/// A null cascade means it has not loaded yet; a null <paramref name="User"/> means nobody is selected.
/// </summary>
/// <param name="User">The acting user, or null.</param>
public sealed record ActingUser(UserDto? User);
