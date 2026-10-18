using System.Collections.Concurrent;
using System.Net;

namespace BlazorApp.Stress.Tests;

/// <summary>How a client picks a server for each request (<c>--routing</c>, phase 6c).</summary>
public sealed class RoutingTests
{
    [Fact]
    public async Task Random_routing_spreads_one_users_requests_over_every_server()
    {
        var (api, servers) = Servers();
        var counts = new ConcurrentDictionary<string, int>();
        var client = new ApiClient(servers, "a", new Random(1), counts);

        var answeredBy = new HashSet<string>();
        for (var i = 0; i < 40; i++)
        {
            await client.ListAsync(CancellationToken.None);
            answeredBy.Add(client.Server);
        }

        // Both servers were used, the client names the one that answered, and its home stays put.
        Assert.Equal(["one", "two"], api.Hosts.Keys.Order());
        Assert.Equal(["one", "two"], answeredBy.Order());
        Assert.Equal(40, counts["one"] + counts["two"]);
        Assert.Equal(api.Hosts["one"], counts["one"]);
        Assert.Equal("one", client.Home);
    }

    [Fact]
    public async Task Sticky_routing_keeps_one_users_requests_on_its_own_server()
    {
        var (api, servers) = Servers();
        var counts = new ConcurrentDictionary<string, int>();
        var client = new ApiClient(servers, "a", random: null, counts);

        for (var i = 0; i < 40; i++)
        {
            await client.ListAsync(CancellationToken.None);
        }

        Assert.Equal(["one"], api.Hosts.Keys);
        Assert.Equal(40, counts["one"]);
        Assert.Equal(("one", "one"), (client.Server, client.Home));
    }

    private static (CountingApi Api, (HttpClient Http, string Name)[] Servers) Servers()
    {
        var api = new CountingApi();
        return (api, [(new HttpClient(api) { BaseAddress = new Uri("http://one/") }, "one"), (new HttpClient(api) { BaseAddress = new Uri("http://two/") }, "two")]);
    }

    /// <summary>Answers every request with an empty list and counts requests per host.</summary>
    private sealed class CountingApi : HttpMessageHandler
    {
        public ConcurrentDictionary<string, int> Hosts { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Hosts.AddOrUpdate(request.RequestUri!.Host, 1, (_, n) => n + 1);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]", System.Text.Encoding.UTF8, "application/json") });
        }
    }
}
