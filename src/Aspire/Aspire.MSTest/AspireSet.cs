namespace NeverTest.Aspire;

using Microsoft.Extensions.Logging;
using MSTest;

/// <summary>
/// MSTest test data source for running Aspire based scenarios
/// </summary>
public class AspireSet : ScenarioSetAttribute<AspireState>
{
    /// <summary>
    /// Creates new set based on specified yaml file
    /// </summary>
    /// <param name="set">Path to yaml scenario file</param>
    /// <param name="defaultLogLevel">Default logging level for this scenario set.</param>
    public AspireSet(string set, LogLevel defaultLogLevel = LogLevel.Information) : base(set)
    {
        Builder
            .UseYaml()
            .UseDefaultEngine()
            .Verbosity(defaultLogLevel)
            .Arranges
            .Always<StartAspire>();
    }
}
