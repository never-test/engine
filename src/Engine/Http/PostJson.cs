namespace NeverTest.Http;

using Acts;

[Act("postJson")]
internal class PostJson(
    IHttpClientFactory clientFactory,
    JsonResponseDeserializer deserializer
) : Post(clientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = await SendPost(input, context);
        return await deserializer.Deserialize(response);
    }
}
