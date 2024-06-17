namespace NeverTest.Asserts;

using FluentAssertions.Json;

[Assert("equals")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Equals : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        return JToken
            .FromObject(expected!)
            .Should()
            .BeEquivalentTo(actual)
            .AsTask();
    }
}
