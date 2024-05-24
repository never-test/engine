using NeverTest.StandardScenarios.Yaml;

namespace NeverTest.MSTest;

using Microsoft.Extensions.Logging;

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
