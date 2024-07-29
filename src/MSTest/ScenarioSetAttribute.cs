using Microsoft.Extensions.Logging;

namespace NeverTest.MSTest;

public class ScenarioSetAttribute : ScenarioSetAttribute<Empty>
{
    public ScenarioSetAttribute(string set, LogLevel defaultLevel = LogLevel.Debug) : base(set)
    {
        Builder
            .UseYaml()
            .UseDefaultEngine()
            .Verbosity(defaultLevel);
    }
}
