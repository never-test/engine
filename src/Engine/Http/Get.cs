namespace NeverTest.Http;

using Acts;
using Logging;

[Act("get")]
internal class Get(IHttpClientFactory httpClientFactory) : IActStep<JToken, IState>
{
    public virtual async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context) => await SendGet(input, context);

    protected async Task<HttpResponseMessage> SendGet(JToken input, IScenarioContext<IState> context)
    {
        var serializer = context.JsonSerializer();

        HttpOptions options = input switch
        {
            JValue value => new HttpOptions {Url = new Uri(value.Value<string>()!)},
            JObject obj => obj.ToObject<HttpOptions>(serializer)!,
            _ => throw new NotSupportedException($"{input.Type} is not supported as get options at  {input.Path}")
        };
        context.Debug(options);

        var client = httpClientFactory.CreateClient(options.Name);

        var message = new HttpRequestMessage(HttpMethod.Get, options.Url);

        foreach (var header  in options.GetHeaders())
        {
            message.Headers.Add(header.Name, header.Values);
        }
        var response = await client.SendAsync(message);
        return response;
    }
}
