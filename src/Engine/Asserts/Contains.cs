using FluentAssertions;

namespace NeverTest.Asserts;

using FluentAssertions.Json;

[Assert("contains")]
// ReSharper disable once ClassNeverInstantiated.Global
public class Contains : IAssertStep
{
    public ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        if (actual is JValue { Type: JTokenType.String } jvs &&
            expected is JValue { Type: JTokenType.String } exp)
        {
            var actualString = jvs.Value<string>();
            var expectedString = exp.Value<string>();

           return actualString
                   .Should()
                   .Contain(expectedString)
                   .AsTask();
        }

        return JToken
            .FromObject(actual!)
            .Should()
            .ContainSubtree(expected)
            .AsTask();
    }
}
