namespace NeverTest;

public interface IActStep<in TInput, in TState> where TState : IState
{
    public Task<object?> Act(TInput input, IScenarioContext<TState> context);
}
