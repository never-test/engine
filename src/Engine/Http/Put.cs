namespace NeverTest.Http;

using Acts;
using Logging;

[Act("put")]
internal class Put(IHttpClientFactory clientFactory): HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context)
    {
        return await SendPut(input, context);
    }

    protected Task<HttpResponseMessage> SendPut(JToken input, IScenarioContext<IState> context)
    {
        var options = HttpOptions.FromToken(
            input,
            context.JsonSerializer());

        var raw = options.Body?.ToString();

        context.Dump(raw);

        var content = raw is not null ? new StringContent(raw) : null;

        return Send(
            HttpMethod.Put,
            options,
            content,
            context);
    }
}
