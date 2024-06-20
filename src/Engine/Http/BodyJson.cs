using NeverTest.Logging;

namespace NeverTest.Http;

using Asserts;


[Assert("bodyJson")]
internal class BodyJson(JsonResponseDeserializer deserializer) : IAssertStep
{

    public async ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        var response = context.GetOutput<HttpResponseMessage>(actual!)!;
        var body = await deserializer.Deserialize(response);
        context.Trace(body);

        await context.ExecuteAssertToken(expected!,  body);
    }
}
