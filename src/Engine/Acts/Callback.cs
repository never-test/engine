namespace NeverTest.Acts;

internal class Callback<TInput, TState> : IActStep<TInput, TState> where TState : IState
{
    private readonly Func<TInput, IScenarioContext<TState>, object?> _callback;

    // public Callback(Func<TInput, object?> callback)
    // {
    //     ArgumentNullException.ThrowIfNull(callback);
    //     _callback = (input, context) => callback(input);
    // }public Callback(Func<TInput, TState, object?> callback)
    // {
    //     ArgumentNullException.ThrowIfNull(callback);
    //     _callback = (input, context) => callback(input, context.State());
    // }
    public Callback(Func<TInput, IScenarioContext<TState>, object?> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = callback;
    }
    public Task<object?> Act(TInput input, IScenarioContext<TState> context) => Task.FromResult(_callback(input, context));
}
