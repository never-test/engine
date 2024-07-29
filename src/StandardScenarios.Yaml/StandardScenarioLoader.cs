using Newtonsoft.Json;

namespace NeverTest.StandardScenarios.Yaml;

using Microsoft.Extensions.Options;
using YamlConverter;
using YamlDotNet.Serialization;

using NeverTest.Yaml;

public class StandardScenarioLoader(IOptions<YamlOptions> options) : IScenarioSetLoader
{
    public ScenarioSetBase<T> Load<T>(string scenarioSet) where T : IState
    {
        var scenarioStream = GetType().Assembly.GetManifestResourceStream(scenarioSet);
        if (scenarioStream is null)
        {
            throw new InvalidOperationException($"Unable to load scenario: {scenarioSet}");
        }

        var builder = new DeserializerBuilder()
            .WithTypeConverter(new JTokenYamlConverter())
            .WithNamingConvention(options.Value.NamingConvention);

        options.Value.Customize?.Invoke(builder);

        var deserializer = builder.Build();
        using var reader = new StreamReader(scenarioStream);

        var set = deserializer.Deserialize<ScenarioSet<T>>(reader);
        set.Name ??= scenarioSet;
        return set;
    }
}
