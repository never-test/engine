namespace NeverTest.Yaml;

public interface IScenarioSetLoader
{
    public ScenarioSet<T> Load<T>(string set) where T : IState;
}
