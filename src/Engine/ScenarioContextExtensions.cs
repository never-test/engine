namespace NeverTest;

public static class ScenarioContextExtensions
{
    public static JsonSerializer JsonSerializer(this IScenarioContext context) => Newtonsoft.Json.JsonSerializer.Create(context.JsonSerializerSettings);
}
