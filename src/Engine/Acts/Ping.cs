namespace NeverTest.Acts;


public class Ping : IActStep<JToken?>
{
    public Task<object?> Act(JToken? input, IScenarioContext<IState> context)
    {

        return Task.FromResult<object?>("pong");
    }
}