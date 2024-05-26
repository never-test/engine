using System.Runtime.ExceptionServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NeverTest.MSTest;

/// <summary>
/// Scenario runner using default empty state.
/// Should
/// </summary>
public class Runner : Runner<State>
{
    protected override Task<State> CreateState(JToken? state) => State.Instance;
}
public abstract class Runner<T> where T : IState
{
#nullable disable
    public TestContext TestContext { get; set; }
#nullable enable

    protected async Task Run(Scenario<T> scenario)
    {
        var result = await scenario.Run(CreateState);

        TestContext.WriteLine(result.GetHeader());

        if (!string.IsNullOrEmpty(scenario.Inconclusive))
        {
            Assert.Inconclusive(scenario.Inconclusive);
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
