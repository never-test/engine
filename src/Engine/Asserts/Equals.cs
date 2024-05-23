using FluentAssertions.Json;

namespace NeverTest.Asserts;

public class Equals : IAssertStep 
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context) => JToken
        .FromObject(actual!)
        .Should()
        .BeEquivalentTo(expected)
        .End();
}

// context.Actual<HttpResponseMessage>().StatusCode