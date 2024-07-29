namespace NeverTest.Yaml;

public interface IScenarioSetLoader
{
    public ScenarioSetBase<T> Load<T>(string scenarioSet) where T : IState;
}
