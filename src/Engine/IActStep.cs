namespace NeverTest;

public interface IActStep<in TInput, TOutput, in TState> where TState : IState
{
    public Task<TOutput> Act(TInput input, IScenarioContext<TState> context);
}

public interface IActStep<in TInput, TOutput>: IActStep<TInput,TOutput, IState>
{
   
}
public interface IActStep<in TInput>: IActStep<TInput, object?>
{
   
}

