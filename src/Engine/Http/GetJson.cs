using NeverTest.Acts;

namespace NeverTest.Http;

[Act("getJson")]
// ReSharper disable once ClassNeverInstantiated.Global
internal class GetJson(
    IHttpClientFactory httpClientFactory,
    JsonResponseDeserializer deserializer
) : Get(httpClientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = await SendGet(input, context);
        return await deserializer.Deserialize(response);
    }
}
