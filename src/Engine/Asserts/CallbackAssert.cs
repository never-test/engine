namespace NeverTest.Asserts;

internal class CallbackAssert<TState> : IAssertStep<TState> where TState : IState
{
    private readonly Action<JToken?, JToken?,IScenarioContext<TState>> _callback;

    public CallbackAssert(Action<JToken?,JToken?, IScenarioContext<TState>> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _callback = callback;
    }

    public  ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<TState> context)
    {
        _callback(actual, expected, context);
        return ValueTask.CompletedTask;
    }
}
