using NeverTest.Acts;

namespace NeverTest.Http;

[Act("getJson")]
// ReSharper disable once ClassNeverInstantiated.Global
internal class GetJson(
    IHttpClientFactory clientFactory,
    JsonResponseDeserializer deserializer
) : HttpBase(clientFactory)
{
    public override async Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        var response = await Send(
            HttpMethod.Get,
            HttpOptions.FromToken(input, context.JsonSerializer()),
            null,
            context);

        return await deserializer.Deserialize(response);
    }
}
