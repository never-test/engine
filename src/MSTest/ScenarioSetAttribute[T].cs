namespace NeverTest.MSTest;

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public abstract class ScenarioSetAttribute<T>(string path) : TestMethodAttribute, ITestDataSource
    where T : IState
{
    protected ScenarioBuilder<T> Builder { get; init; } = new();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var engine = Builder.Build(GetType().AssemblyQualifiedName!);

        var set = engine.LoadSet<T>(path);

        foreach (var scenario in set.Scenarios)
        {
            yield return [scenario];
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        return ((Scenario<T>)data[0]).Name;
    }
}
