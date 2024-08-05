namespace NeverTest.AspNet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

public class AppState<TEntryPoint> : IState where TEntryPoint : class
{
    private readonly List<Action<IWebHostBuilder>> _actions = new();
    public required HostOptions Options { get; init; }
    private readonly Lazy<WebApplicationFactory<TEntryPoint>> _factory;

    public WebApplicationFactory<TEntryPoint> Factory => _factory.Value;

    public AppState()
    {
        _factory = new Lazy<WebApplicationFactory<TEntryPoint>>(Build);
    }

    private WebApplicationFactory<TEntryPoint> Build() => new WebApplicationFactory<TEntryPoint>()
        .WithWebHostBuilder(builder =>
        {
            foreach (var action in _actions)
            {
                action(builder);
            }
        });

    public void Configure(Action<IWebHostBuilder> configure)
    {
        if (_factory.IsValueCreated)
        {
            var message = "Cannot set setting because host is already created." +
                          "Are you trying to set setting in a scenario when mode is set to shared?";
            throw new InvalidOperationException(message);
        }
        _actions.Add(configure);
    }
}
