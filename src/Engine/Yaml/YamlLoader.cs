using Microsoft.Extensions.Options;
using YamlConverter;
using YamlDotNet.Serialization;

namespace NeverTest.Yaml;

public class YamlLoader(IOptions<YamlOptions> options) : IScenarioSetLoader
{
    private readonly YamlOptions _options = options.Value;
    public ScenarioSetBase<T> Load<T>(string scenarioSet) where T : IState
    {
        var builder = new DeserializerBuilder()
           .WithTypeConverter(new JTokenYamlConverter())
           .WithNamingConvention(_options.NamingConvention);

        _options?.Customize?.Invoke(builder);

        var deserializer = builder.Build();
        var plain = File.ReadAllText(scenarioSet);
        var set = deserializer.Deserialize<ScenarioSetBase<T>>(plain);

        set.Name ??= scenarioSet;

        return set;
    }
}
