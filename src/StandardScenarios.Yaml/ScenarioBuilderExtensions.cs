namespace NeverTest.StandardScenarios.Yaml;

using NeverTest.Yaml;
using Microsoft.Extensions.DependencyInjection;

public static class ScenarioBuilderExtensions
{
    public static ScenarioBuilder<Empty> UseStandardScenarioSets(this ScenarioBuilder<Empty> builder)
    {
        builder.Services.AddOptions<YamlOptions>();
        builder.Services.AddSingleton<IScenarioSetLoader, StandardScenarioLoader>();

        return builder;
    }
}
