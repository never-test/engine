namespace NeverTest.Http;

using Acts;


[Act("get")]
internal class Get(IHttpClientFactory clientFactory) :
    HttpBase(clientFactory)
{
    public override async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context) => await Send(
        HttpMethod.Get,
        HttpOptions.FromToken(input, context.JsonSerializer()),
        null,
        context);
}
