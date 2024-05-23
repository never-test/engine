namespace NeverTest.MSTest;

using Microsoft.Extensions.Logging;
using NeverTest.Scenarios.Yaml;

public class StandardSetAttribute : ScenarioSetAttribute<State>
{
    public StandardSetAttribute(string set) : base(set)
    {
        Builder
            .UseStandardScenarioSets()
            .UseDefaultEngine()
            .Verbosity(LogLevel.Debug);
    }
}
