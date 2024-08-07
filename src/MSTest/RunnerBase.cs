using System.Runtime.ExceptionServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NeverTest.MSTest;

/// <summary>
/// Scenario runner using default empty state.
/// </summary>
public class DefaultRunner : RunnerBase<Empty>
{
    protected override Task<Empty> CreateState(JToken? state) => Empty.Instance;
}

public abstract class RunnerBase<T> where T : IState
{
#nullable disable
    public TestContext TestContext { get; set; }
#nullable enable

    protected async Task Run(Scenario scenario)
    {
        var typedScenario = (Scenario<T>)scenario;
        var result = await typedScenario.Run(CreateState);

        TestContext.WriteLine(result.GetHeader());

        if (!string.IsNullOrEmpty(typedScenario.Inconclusive))
        {
            Assert.Inconclusive(typedScenario.Inconclusive);
        }

        foreach (var log in result.Logs)
        {
            TestContext.WriteLine(log);
        }

        if (result.Exception is not null)
        {
            ExceptionDispatchInfo.Capture(result.Exception).Throw();
        }
    }
    protected abstract Task<T> CreateState(JToken? state);
}

