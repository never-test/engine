namespace NeverTest.MSTest;

using NeverTest.StandardScenarios.Yaml;
using Microsoft.Extensions.Logging;

/// <summary>
/// Allows running standard scenario set embedded in StandardScenarios.Yaml assembly
/// using MSTest.
/// </summary>
public class StandardSetAttribute : ScenarioSetAttribute<Empty>
{
    /// <summary>
    /// Creates new instance StandardSetAttribute class
    /// </summary>
    /// <param name="set">Name of the standard set. See
    /// <see cref="NeverTest.StandardScenarios.Yaml.Sets"/>
    /// for full list of predefined standard sets.</param>
    public StandardSetAttribute(string set) : base(set)
    {
        Builder
            .UseStandardScenarioSets()
            .UseDefaultEngine()
            .Verbosity(LogLevel.Debug);
    }
}
