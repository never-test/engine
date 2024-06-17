namespace NeverTest.Yaml;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class YamlOptions
{
    public INamingConvention NamingConvention { get; set; } = UnderscoredNamingConvention.Instance;
    public Action<DeserializerBuilder>? Customize { get; set; }

}
