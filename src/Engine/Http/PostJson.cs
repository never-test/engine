namespace NeverTest.Http;

using Acts;

[Act("postJson")]
internal class PostJson(
    IHttpClientFactory httpClientFactory,
    JsonResponseDeserializer deserializer
) : Post(httpClientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = (HttpResponseMessage?)await base.Act(input, context);
        ArgumentNullException.ThrowIfNull(response);
        return await deserializer.Deserialize(response);
    }
}
