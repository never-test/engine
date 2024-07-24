namespace NeverTest.Http;

using Acts;
using Logging;

[Act("patch")]
internal class Patch(IHttpClientFactory clientFactory): HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context)
    {
        return await SendPatch(input, context);
    }

    protected Task<HttpResponseMessage> SendPatch(JToken input, IScenarioContext<IState> context)
    {
        var options = HttpOptions.FromToken(
            input,
            context.JsonSerializer());

        var raw = options.Body?.ToString();

        context.Dump(raw);

        var content = raw is not null ? new StringContent(raw) : null;

        return Send(
            HttpMethod.Patch,
            options,
            content,
            context);
    }
}
