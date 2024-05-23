namespace NeverTest.Acts;

public class Echo : IActStep<JToken, IState>
{
    public Task<object?> Act(JToken input, IScenarioContext<IState> context)
    {
        return Task.FromResult<object?>(input);
    }
}
