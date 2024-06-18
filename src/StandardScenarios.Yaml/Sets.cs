namespace NeverTest.StandardScenarios.Yaml;

/// <summary>
/// Contains list of predefines scenario set names.
/// </summary>
public static class Sets
{
    public static class Acts
    {
        public const string Basics = "NeverTest.StandardScenarios.Yaml.acting.1.basics.yaml";
        public const string Echo = "NeverTest.StandardScenarios.Yaml.acting.2.echo.yaml";
        public const string Folding = "NeverTest.StandardScenarios.Yaml.acting.3.folding.yaml";
        public const string Naming = "NeverTest.StandardScenarios.Yaml.acting.4.naming.yaml";
        public const string Nesting = "NeverTest.StandardScenarios.Yaml.acting.5.nesting.yaml";
        public const string Referencing = "NeverTest.StandardScenarios.Yaml.acting.6.referencing.yaml";
        public const string Advanced = "NeverTest.StandardScenarios.Yaml.acting.666.advanced.yaml";
    }

    public static class Asserts
    {
        public const string Exceptions = "NeverTest.StandardScenarios.Yaml.asserting.0.exceptions.yaml";
        public const string Basics = "NeverTest.StandardScenarios.Yaml.asserting.1.basics.yaml";
        public const string Referencing = "NeverTest.StandardScenarios.Yaml.asserting.2.referencing.yaml";

    }
    public static class Http
    {
        public const string Get = "NeverTest.StandardScenarios.Yaml.http.1.get.yaml";
    }

    public static IEnumerable<string> All => typeof(Sets).Assembly.GetManifestResourceNames();


}
