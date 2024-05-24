namespace NeverTest.Yaml;

public interface IScenarioSetLoader
{
    public ScenarioSetBase<T> Load<T>(string set) where T : IState;
}
