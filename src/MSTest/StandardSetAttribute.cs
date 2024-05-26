namespace NeverTest.MSTest;

using NeverTest.StandardScenarios.Yaml;
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

