namespace NeverTest.AspNet;

using MSTest;
using Newtonsoft.Json.Linq;

/// <summary>
/// Allows executing ASP.NET based scenarios
/// </summary>
/// <typeparam name="TEntryPoint">ASP.NET entry point. Usually Program.</typeparam>
/// <remarks>
/// SUT and Test project needs to follow requirements described in
/// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
/// <br />
/// 1. Specify the Web SDK in the project file
/// <br />
/// 2. Expose internal types from the web app to the test project
/// <br />
/// 3. Define Program in the ASP.NET app which will be used to reference entry point
/// </remarks>
public class Runner<TEntryPoint> : RunnerBase<AppState<TEntryPoint>> where TEntryPoint : class
{
    protected override Task<AppState<TEntryPoint>> CreateState(JToken? state)
    {
        var hostOptions = state is null ? new HostOptions() : state.ToObject<HostOptions>();
        ArgumentNullException.ThrowIfNull(hostOptions);

        var appState = new AppState<TEntryPoint>
        {
            Options = hostOptions
        };

        if (hostOptions.Settings.Count > 0)
        {
            appState
                .Factory
                .WithWebHostBuilder(builder =>
                {
                    foreach (var (key, value) in hostOptions.Settings)
                    {
                        builder.UseSetting(key, value);
                    }
                });
        }

        return Task.FromResult(appState);
    }
}
