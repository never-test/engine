namespace NeverTest;

public interface IState;

public sealed class State : IState
{
    public static readonly Task<State> Instance = Task.FromResult(new State());
    
}



