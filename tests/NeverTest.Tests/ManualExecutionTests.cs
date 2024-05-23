using FluentAssertions;
using NeverTest.Acts;
using NeverTest.Scenarios.Yaml;
using Newtonsoft.Json.Linq;

namespace NeverTest.Tests;

[TestClass]
public class ManualExecutionTests
{
    [TestMethod]
    public async Task Should_run_all_predefined_scenarios_manually()
    {
        var builder = new ScenarioBuilder<State>()
            .UseDefaultEngine()
            .UseStandardScenarioSets();

        var engine = builder.Build();

        foreach (var setName in StandardSet.Names())
        {
            var set = engine.LoadScenarios<State>(setName);

            foreach (var scenario in set)
            {
                var result = await scenario.Run(() => State.Instance);
                result.Exception.Should().BeNull();
            }
        }
    }

    [DataTestMethod]
    [DataRow(Sets.Basics)]
    [DataRow(Sets.Echo)]
    [DataRow(Sets.Referencing)]
    [DataRow(Sets.Naming)]
    [DataRow(Sets.Advanced)]
    [DataRow(Sets.Nesting)]
    [DataRow(Sets.Folding)]
    public async Task Should_run_predefined_set(string name)
    {
        var builder = new ScenarioBuilder<State>()
            .UseDefaultEngine()
            .UseStandardScenarioSets();

        var engine = builder.Build();

        var set = engine.LoadScenarios<State>(name);

        foreach (var scenario in set)
        {
            var result = await scenario.Run(() => State.Instance);
            result.Exception.Should().BeNull();
        }
    }
}
