namespace NeverTest.Http;

public class HttpOptions
{
    public required Uri Url { get; init; }
    public string Name { get; init; } = Microsoft.Extensions.Options.Options.DefaultName;
    public JToken? Body { get; init; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public Headers? Headers { get; set; }

    public IEnumerable<(string Name, IEnumerable<string> Values)> GetHeaders()
    {
        if(Headers is null) yield break;
        foreach (var header in Headers)
        {
            switch (header.Value)
            {
                case JArray ja:
                    yield return (header.Key, ja.ToObject<string[]>()!);
                    break;

                case JValue jv:
                    yield return (header.Key, new[] {jv.Value<string>()!});
                    break;
                default:
                    throw new NotSupportedException($"'{header.Value.Type}' is not supported as header input.");
            }
        }
    }

    public static HttpOptions FromToken(JToken token, JsonSerializer serializer) => token switch
    {
        JValue value => new HttpOptions {Url = new Uri(value.Value<string>()!)},
        JObject obj => obj.ToObject<HttpOptions>(serializer)!,
        _ => throw new NotSupportedException($"{token.Type} is not supported as get options at  {token.Path}")
    };
}
