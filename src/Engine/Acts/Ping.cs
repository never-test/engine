namespace NeverTest.Acts;

// public class Ping<TState> : IActStep<JToken?, TState> where TState : IState
// {
//     public Task<object?> Act(JToken? input, IScenarioContext<TState> context)
//     {
//         return Task.FromResult<object?>("pong");
//     }
// }


public class Ping : IActStep<JToken?, IState>
{
    public Task<object?> Act(JToken? input, IScenarioContext<IState> context)
    {
        return Task.FromResult<object?>("pong");
    }
}
