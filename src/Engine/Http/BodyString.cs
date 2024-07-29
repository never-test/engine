using NeverTest.Logging;


namespace NeverTest.Http;

using Asserts;


[Assert("bodyString")]
internal class BodyString : IAssertStep
{
    public async ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        var response = context.GetOutput<HttpResponseMessage>(actual!)!;
        var str = await response.Content.ReadAsStringAsync();

        await context.ExecuteAssertToken(expected!,  JValue.CreateString(str));
    }
}
