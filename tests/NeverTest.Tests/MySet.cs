namespace NeverTest.Tests;

using Microsoft.Extensions.Logging;

public class MySet : ScenarioSetAttribute<MyScenarioState>
{
    public MySet(string set) : base(set)
    {
        Builder
            .UseYaml()
            .UseDefaultEngine()
            .Verbosity(LogLevel.Debug);
    }
}
