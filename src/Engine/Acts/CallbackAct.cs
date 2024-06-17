namespace NeverTest.Acts;

internal class CallbackAct<TInput, TState> : IActStep<TInput, TState> where TState : IState
{
    private readonly Func<TInput, IScenarioContext<TState>, object?> _callback;
    public CallbackAct(Func<TInput, IScenarioContext<TState>, object?> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = callback;
    }
    public Task<object?> Act(TInput input, IScenarioContext<TState> context) => Task.FromResult(_callback(input, context));
}
