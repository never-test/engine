namespace NeverTest.Scenarios.Yaml;

/// <summary>
/// Contains list of predefines scenario set names.
/// </summary>
public static class Sets
{
    public const string Basics = "NeverTest.Scenarios.Yaml.acting.1.basics.yaml";
    public const string Echo = "NeverTest.Scenarios.Yaml.acting.2.echo.yaml";
    public const string Folding = "NeverTest.Scenarios.Yaml.acting.3.folding.yaml";
    public const string Naming = "NeverTest.Scenarios.Yaml.acting.4.naming.yaml";
    public const string Nesting = "NeverTest.Scenarios.Yaml.acting.5.nesting.yaml";
    public const string Referencing = "NeverTest.Scenarios.Yaml.acting.6.referencing.yaml";
    public const string Advanced = "NeverTest.Scenarios.Yaml.acting.666.advanced.yaml";

    public static IEnumerable<string> All => typeof(Sets).Assembly.GetManifestResourceNames();
}
