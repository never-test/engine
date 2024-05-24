namespace NeverTest;

public interface IArrangeStep<in TOptions, in TState> where TState : IState
{
    Task Arrange(TOptions options, IScenarioContext<TState> context);
}

