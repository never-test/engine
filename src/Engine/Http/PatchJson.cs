namespace NeverTest.Http;

using Acts;

[Act("patchJson")]
internal class PatchJson(
    IHttpClientFactory clientFactory,
    JsonResponseDeserializer deserializer
) : Patch(clientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = await SendPatch(input, context);
        return await deserializer.Deserialize(response);
    }
}
