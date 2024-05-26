using NeverTest.Tests.CustomState;

namespace NeverTest.Tests;

using NeverTest.StandardScenarios.Yaml;

[TestClass]
public class ManualExecutionTests
{
    [TestMethod]
    public async Task Should_run_all_predefined_scenarios_manually()
    {
        var engine = new ScenarioBuilder<State>()
            .UseDefaultEngine()
            .UseStandardScenarioSets()
            .Build();

        foreach (var setName in Sets.All)
        {
            var result = await engine
                .LoadSet<State>(setName)
                .Run(_ => State.Instance);

            result.Assert();
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
        var result = await new ScenarioBuilder<State>()
            .UseDefaultEngine()
            .UseStandardScenarioSets()
            .Build()
            .LoadSet<State>(name)
            .Run(_ => State.Instance);

        result.Assert();
    }
}
