using BlazorApp.Shared.Contracts;

namespace BlazorApp.Server.Logging;

/// <summary>
/// <see cref="IClientEventLog"/> for components while the server prerenders them. Nothing to
/// log: no button can be pressed before the page is interactive, and page loads are logged by
/// the browser once WebAssembly has taken over.
/// </summary>
public sealed class PrerenderClientEventLog : IClientEventLog
{
    /// <inheritdoc />
    public string? CurrentCorrelationId => null;

    /// <inheritdoc />
    public void PageLoaded(string page, Guid? recordId = null, string? result = null)
    {
    }

    /// <inheritdoc />
    public IClientAction ButtonPressed(string page, string action, Guid? recordId = null) => new NoAction();

    /// <inheritdoc />
    public void ApiFailed(string method, string path, int statusCode)
    {
    }

    /// <inheritdoc />
    public Task FlushAsync() => Task.CompletedTask;

    private sealed class NoAction : IClientAction
    {
        public string CorrelationId { get; } = Shared.Contracts.CorrelationId.New();

        public void Result(string result)
        {
        }

        public void Dispose()
        {
        }
    }
}
