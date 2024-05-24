namespace NeverTest.StandardScenarios.Yaml;

using NeverTest.Yaml;
using YamlConverter;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class StandardScenarioLoader : IScenarioSetLoader
{
    public ScenarioSetBase<T> Load<T>(string set) where T : IState
    {
        var scenarioStream = GetType().Assembly.GetManifestResourceStream(set);
        if (scenarioStream is null)
        {
            throw new InvalidOperationException($"Unable to load scenario: {set}");
        }

        var builder = new DeserializerBuilder()
            .WithTypeConverter(new JTokenYamlConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance);

        var deserializer = builder.Build();
        using var reader = new StreamReader(scenarioStream);

        var scenarioSet = deserializer.Deserialize<ScenarioSet<T>>(reader);
        scenarioSet.Name ??= set;
        return scenarioSet;
    }
}
