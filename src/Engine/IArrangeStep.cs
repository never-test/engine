namespace NeverTest;

public interface IArrangeStep<in TOptions, in TState> where TState : IState
{
    Task Arrange(TOptions options, IArrangeScenarioContext<TState> context);
}

public interface IArrangeStep<in TState> where TState : IState
{
    Task Arrange(IArrangeScenarioContext<TState> context);
}
