namespace NeverTest;

public interface IAlwaysArrangeStep<T> where T: IState
{
    Task Arrange(ScenarioContext<T> scenarioContext);
}