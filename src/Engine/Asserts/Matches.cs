namespace NeverTest.Asserts;

using FluentAssertions.Json;

[Assert("matches")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Matches : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context) => JToken
        .FromObject(actual!)
        .Should()
        .ContainSubtree(expected)
        .AsTask();
}
