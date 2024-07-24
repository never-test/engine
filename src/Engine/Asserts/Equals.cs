using System.Diagnostics.CodeAnalysis;
using FluentAssertions;

namespace NeverTest.Asserts;

using FluentAssertions.Json;

[Assert("equals")]
// ReSharper disable once ClassNeverInstantiated.Global
internal class Equals : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        if (TokenTypeMatch<string>(JTokenType.String, out var stringResult))
        {
            var actualString = stringResult.Value.Actual;

            actualString
                .Should()
                .Be(stringResult.Value.Expeceted, "'equals' assert is defined at '{0}'", expected?.Path);

            return ValueTask.CompletedTask;
        }

        if (TokenTypeMatch<int>(JTokenType.Integer, out var intResult))
        {
            var actualInt = intResult.Value.Actual;

            actualInt
                .Should()
                .Be(intResult.Value.Expeceted, "'equals' assert is defined at '{0}'", expected?.Path);

            return ValueTask.CompletedTask;
        }

        return JToken
            .FromObject(expected!)
            .Should()
            .BeEquivalentTo(actual)
            .AsTask();

        bool TokenTypeMatch<T>(
            JTokenType type,
            [NotNullWhen(true)] out (T Actual, T Expeceted)? result)
        {
            if (actual?.Type == type && expected?.Type == type)
            {
                result = (actual.Value<T>()!, expected.Value<T>()!);
                return true;
            }

            result = null;
            return false;
        }
    }
}
