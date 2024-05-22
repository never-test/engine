using FluentAssertions;
using NeverTest.Acts;
using NeverTest.Scenarios.Yaml;
using Newtonsoft.Json.Linq;

namespace NeverTest.Tests;

[TestClass]
public class ManualExecution
{
    [TestMethod]
    public async Task AllStandardSetScenarioLoadAndRun()
    {
        // TODO: investigate how to move Add* to generic extension
        var builder = new ScenarioBuilder<State>()
            .AddAct<Ping>("ping")
            .AddAct<Echo>("echo")
            .AddAct<Repeat, JObject>("repeat")
            .UseStandardScenarios();
        
        var engine = builder.Build();

        var setNames = StandardSet.Names();
        foreach (var setName in setNames)
        {
            var set = engine.LoadScenarios<State>(setName);

            foreach (var scenario in set)
            {
                var result = await scenario.Run(() => State.Instance);
                result.Exception.Should().BeNull();
            }
        }
    }
}