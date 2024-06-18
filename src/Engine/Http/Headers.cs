using System.Net.Http.Json;
using NeverTest.Asserts;

namespace NeverTest.Http;

public class Headers : Dictionary<string, JToken>;



[Assert("jsonBody")]
public class JsonBody : IAssertStep
{
    public async ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        var response = context.GetOutput<HttpResponseMessage>(actual!)!;

        var bodyAsString = await response.Content.ReadAsStringAsync();
        await context.ExecuteAssertToken(expected!, JToken.Parse(bodyAsString));

    }
}
