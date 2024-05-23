
namespace NeverTest.Asserts;
using FluentAssertions.Json;

public class Nil: IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context) =>
        actual
            .Should()
            .NotBeNull()
            .End();
}