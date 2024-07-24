namespace NeverTest.Http;

using Acts;
using Logging;

[Act("post")]
internal class Post(IHttpClientFactory clientFactory): HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context)
    {
        return await SendPost(input, context);
    }

    protected Task<HttpResponseMessage> SendPost(JToken input, IScenarioContext<IState> context)
    {
        var options = HttpOptions.FromToken(
            input,
            context.JsonSerializer());

        var raw = options.Body?.ToString();

        context.Dump(raw);

        var content = raw is not null ? new StringContent(raw) : null;

        return Send(
            HttpMethod.Post,
            options,
            content,
            context);
    }
}
