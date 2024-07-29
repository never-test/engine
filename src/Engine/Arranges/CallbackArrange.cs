namespace NeverTest.Arranges;

internal class CallbackArrange<TInput, TState> : IArrangeStep<TInput, TState> where TState : IState
{
    private readonly Action<TInput, IArrangeScenarioContext<TState>> _callback;
    public CallbackArrange(Action<TInput, IArrangeScenarioContext<TState>> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = callback;
    }


    public Task Arrange(TInput options, IArrangeScenarioContext<TState> context)
    {
        _callback(options, context);
        return Task.CompletedTask;
    }
}
