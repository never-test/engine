namespace NeverTest.Http;

using Acts;


[Act("get")]
internal class Get(IHttpClientFactory clientFactory) :
    HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context)
    {
        var httpOptions = HttpOptions.FromToken(input, context.JsonSerializer());

        return await Send(
            HttpMethod.Get,
            httpOptions,
            null,
            context);
    }
}
