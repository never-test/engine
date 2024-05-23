using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YamlDotNet.Serialization.NamingConventions;

namespace NeverTest.Scenarios.Yaml;

using NeverTest.Yaml;
using YamlConverter;
using YamlDotNet.Serialization;

public class StandardScenarioLoader : IScenarioSetLoader
{
    public ScenarioSet<T> Load<T>(string set) where T : IState
    {
        var scenarioStream = GetType().Assembly.GetManifestResourceStream(set);
        if (scenarioStream is null)
        {
            throw new InvalidOperationException($"Unable to load scenario: {set}");
        }

        var builder = new DeserializerBuilder()
            .WithTypeConverter(new JTokenYamlConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance);
        ;

        var deserializer = builder.Build();
        using var reader = new StreamReader(scenarioStream);

        var scenarioSet = deserializer.Deserialize<ScenarioSet<T>>(reader);
        scenarioSet.Name ??= set;
        return scenarioSet;
    }
}

public static class StandardSet
{
    private static readonly Dictionary<string, ScenarioSet<State>> _sets;

    static StandardSet()
    {
        _sets = LoadSets().ToDictionary(x => x.Name, x => x);
    }

    public static ScenarioSet<State> GetSet(string name) => _sets[name];
    public static IEnumerable<ScenarioSet<State>> Sets() => _sets.Values;
    public static IEnumerable<string> Names() => _sets.Keys;
    private static IEnumerable<ScenarioSet<State>> LoadSets()
    {
        var loader = new StandardScenarioLoader();
        var setNames = typeof(StandardSet).Assembly.GetManifestResourceNames();
        foreach (var setName in setNames)
        {
            yield return loader.Load<State>(setName); ;
        }
    }
}

public static class Ext
{
    public static ScenarioBuilder<State> UseStandardScenarioSets(this ScenarioBuilder<State> builder)
    {
        builder.Services.AddOptions<YamlOptions>();
        builder.Services.AddSingleton<IScenarioSetLoader, StandardScenarioLoader>();

        return builder;
    }
}
