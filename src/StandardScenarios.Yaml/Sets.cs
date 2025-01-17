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
        public const string EqualsAssert = "NeverTest.StandardScenarios.Yaml.asserting.3.equals.yaml";

    }
    public static class Http
    {
        public const string Get = "NeverTest.StandardScenarios.Yaml.http.1.get.yaml";
        public const string Post = "NeverTest.StandardScenarios.Yaml.http.2.post.yaml";
        public const string Delete = "NeverTest.StandardScenarios.Yaml.http.3.delete.yaml";
        public const string Put = "NeverTest.StandardScenarios.Yaml.http.4.put.yaml";
        public const string Patch = "NeverTest.StandardScenarios.Yaml.http.5.patch.yaml";
        public const string ResponseCodes = "NeverTest.StandardScenarios.Yaml.http.6.response-codes.yaml";
    }

    public static IEnumerable<string> All => typeof(Sets).Assembly.GetManifestResourceNames();
}
