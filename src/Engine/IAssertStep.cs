namespace NeverTest;

public interface IAssertStep<in TState> where TState : IState
{
    ValueTask Assert(JToken actual, JToken? expected, IScenarioContext<TState> context);
    ValueTask AssertException(Exception exception, JToken expected, IScenarioContext<TState> context) => ValueTask.CompletedTask;

}

public interface IAssertStep : IAssertStep<IState>;
