using NeverTest.Tests.CustomState;

namespace NeverTest.Tests;

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
    [DataRow(Sets.Acts.Basics)]
    [DataRow(Sets.Acts.Echo)]
    [DataRow(Sets.Acts.Referencing)]
    [DataRow(Sets.Acts.Naming)]
    [DataRow(Sets.Acts.Advanced)]
    [DataRow(Sets.Acts.Nesting)]
    [DataRow(Sets.Acts.Folding)]
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
