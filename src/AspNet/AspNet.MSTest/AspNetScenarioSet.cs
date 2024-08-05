namespace NeverTest.AspNet;

using Microsoft.Extensions.Logging;
using MSTest;

/// <summary>
/// ASP.NET scenario test data source for MSTest.
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
public class AspNetScenarioSet<TEntryPoint> : ScenarioSetAttribute<AppState<TEntryPoint>> where TEntryPoint : class
{
    /// <summary>
    /// ASP.NET scenario test data source
    /// </summary>
    /// <param name="set">Path to scenario yaml</param>
    /// <param name="defaultLogLevel">Default log level for all scenarios</param>
    public AspNetScenarioSet(string set, LogLevel defaultLogLevel = LogLevel.Information): base(set)
    {
        Builder.AddAspNet(defaultLogLevel);
    }
}
