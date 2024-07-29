namespace NeverTest;

public interface IScenarioContext<out TState> : IScenarioContext where TState : IState
{
    public TState State();
    public Task<ScenarioFrame> ExecuteActToken(JToken token, string output);
    public ValueTask ExecuteAssertToken(JToken token, JToken? actual);
}
