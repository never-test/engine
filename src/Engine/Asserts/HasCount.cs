using FluentAssertions.Json;

namespace NeverTest.Asserts;

[Assert("hasCount")]
internal class HasCount : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        var expectedCount = expected.Value<int>();
        if (actual is JArray jsonArray)
        {
            jsonArray.Should().HaveCount(expectedCount);
            return ValueTask.CompletedTask;
        }

        throw new InvalidOperationException($"'{expected.Path}' expects array but was given {actual.Type}.");
    }
}
