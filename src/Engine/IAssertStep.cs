namespace NeverTest;

public interface IAssertStep<in TState> where TState : IState
{
    ValueTask Assert(JToken? actual, JToken? expected, IScenarioContext<TState> context);
}

public interface IAssertStep : IAssertStep<IState>;
