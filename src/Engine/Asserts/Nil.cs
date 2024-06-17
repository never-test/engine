
namespace NeverTest.Asserts;
using FluentAssertions.Json;

[Assert("nil")]
public class Nil : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context) =>
        actual
            .Should()
            .NotBeNull()
            .AsTask();
}
