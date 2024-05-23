namespace NeverTest;

public interface IScenarioContext<out TState> : IScenarioContext where TState : IState
{
    public TState State();
    public ScenarioFrame Frame { get; }
    public Task<ScenarioFrame> ExecuteActToken(JToken token, string path, string output);
}
