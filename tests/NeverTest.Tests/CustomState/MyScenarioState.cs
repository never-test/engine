namespace NeverTest.Tests.CustomState;

public class MyScenarioState : IState
{
    public int Counter { get; set; }

    public int Increment() => ++Counter;
}
