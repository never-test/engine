namespace NeverTest.StandardScenarios.Yaml;

/// <summary>
/// Contains list of predefines scenario set names.
/// </summary>
public static class Sets
{
    public const string Basics = "NeverTest.StandardScenarios.Yaml.acting.1.basics.yaml";
    public const string Echo = "NeverTest.StandardScenarios.Yaml.acting.2.echo.yaml";
    public const string Folding = "NeverTest.StandardScenarios.Yaml.acting.3.folding.yaml";
    public const string Naming = "NeverTest.StandardScenarios.Yaml.acting.4.naming.yaml";
    public const string Nesting = "NeverTest.StandardScenarios.Yaml.acting.5.nesting.yaml";
    public const string Referencing = "NeverTest.StandardScenarios.Yaml.acting.6.referencing.yaml";
    public const string Advanced = "NeverTest.StandardScenarios.Yaml.acting.666.advanced.yaml";

    public static IEnumerable<string> All => typeof(Sets).Assembly.GetManifestResourceNames();
}
