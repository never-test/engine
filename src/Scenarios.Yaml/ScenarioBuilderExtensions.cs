namespace NeverTest.Scenarios.Yaml;

using Microsoft.Extensions.DependencyInjection;
using NeverTest.Yaml;

public static class ScenarioBuilderExtensions
{
    public static ScenarioBuilder<State> UseStandardScenarioSets(this ScenarioBuilder<State> builder)
    {
        builder.Services.AddOptions<YamlOptions>();
        builder.Services.AddSingleton<IScenarioSetLoader, StandardScenarioLoader>();

        return builder;
    }
}
