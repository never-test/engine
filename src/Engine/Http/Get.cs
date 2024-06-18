namespace NeverTest.Http;

using Acts;
using Logging;

[Act("get")]
internal sealed class Get(IHttpClientFactory httpClientFactory) : IActStep<JToken, IState>
{
    public async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var serializer = context.JsonSerializer();

        GetOptions options = input switch
        {
            JValue value => new GetOptions {Url = new Uri(value.Value<string>()!)},
            JObject obj => obj.ToObject<GetOptions>(serializer)!,
            _ => throw new NotSupportedException($"{input.Type} is not supported as get options at  {input.Path}")
        };
        context.Debug(options);

        var client = httpClientFactory.CreateClient(options.Name);

        var message = new HttpRequestMessage(HttpMethod.Get, options.Url);

        foreach (var header  in options.GetHeaders())
        {
            message.Headers.Add(header.Name, header.Values);
        }

        return await client.SendAsync(message);
    }
}

public class GetOptions
{
    public required Uri Url { get; init; }
    public string Name { get; init; } = Microsoft.Extensions.Options.Options.DefaultName;
    public Headers? Headers { get; set; }

    public IEnumerable<(string Name, IEnumerable<string> Values)> GetHeaders()
    {
        if(Headers is null) yield break;
        foreach (var header in Headers)
        {
            if (header.Value is JArray ja) yield return (header.Key, ja.ToObject<string[]>()!);
            if (header.Value is JValue jv) yield return (header.Key, new[] {jv.Value<string>()!});
        }
    }
}
