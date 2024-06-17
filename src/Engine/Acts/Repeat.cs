namespace NeverTest.Acts;

internal sealed class Repeat : IActStep<JObject, IState>
{
    public async Task<object?> Act(JObject input, IScenarioContext<IState> context)
    {
        var times = input.GetValue(
            "times",
            StringComparison.OrdinalIgnoreCase)?
            .Value<int>();

        var act = input["act"];
        if (act is null) return null;

        var result = new JArray();
        for (var i = 0; i < times; i++)
        {
            var frame = await context.ExecuteActToken(act, i.ToString());
            result.Add(frame.BuildOutput(context) ?? JValue.CreateNull());
        }

        return result;
    }
}
