﻿namespace NeverTest.Asserts;

internal class Select : IAssertStep
{
    private const string PathProperty = "path";

    public async ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<IState> context)
    {
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        if (expected is not JObject selectOptions)
            throw new ArgumentException("Input to 'selected' is expected to be an object with at leas.");

        var path = selectOptions[PathProperty]?.Value<string>() ?? throw new ArgumentException("Assert 'select' requires 'path' property.");

        var input = actual.SelectToken(path);

        foreach (var p in selectOptions.Properties())
        {
            // skip path, treat rest as asserts
            if(p.Name == PathProperty) continue;

            await context.ExecuteAssertToken(p, input);
        }
    }
}
