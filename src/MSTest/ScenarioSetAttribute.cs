using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeverTest.Yaml;

namespace NeverTest.MSTest;

public abstract class ScenarioSetAttribute<T>(string set) : TestMethodAttribute, ITestDataSource
    where T : IState
{
    protected ScenarioBuilder<T> Builder { get; init; } = new();

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var engine = Builder.Build();
        
        foreach (var scenario in engine.LoadScenarios<T>(set))
        {
            yield return [scenario];
        }
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        return ((Scenario<T>)data[0]).Name;
    }
}