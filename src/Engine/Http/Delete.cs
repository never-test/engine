namespace NeverTest.Http;

using Acts;
using Logging;

[Act("delete")]
internal class Delete(IHttpClientFactory clientFactory): HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context) => await SendDelete(input, context);

    private Task<HttpResponseMessage> SendDelete(JToken input, IScenarioContext<IState> context)
    {
        var options = HttpOptions.FromToken(
            input,
            context.JsonSerializer());

        return Send(
            HttpMethod.Delete,
            options,
            null,
            context);
    }
}
