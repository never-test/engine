namespace NeverTest.Http;

using Acts;

[Act("putJson")]
internal class PutJson(
    IHttpClientFactory clientFactory,
    JsonResponseDeserializer deserializer
) : Put(clientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = await SendPut(input, context);
        return await deserializer.Deserialize(response);
    }
}
