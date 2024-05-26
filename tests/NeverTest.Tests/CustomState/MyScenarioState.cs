namespace NeverTest.Tests.CustomState;

public class MyScenarioState : IState
{
    private int _counter;

    public int Counter
    {
        get => _counter;
        init => _counter = value;
    }

    public int Increment() => ++_counter;
}
