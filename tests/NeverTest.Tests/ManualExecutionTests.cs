using NeverTest.Tests.CustomState;

namespace NeverTest.Tests;

[TestClass]
public class ManualExecutionTests
{
    [TestMethod]
    public async Task Should_run_all_predefined_scenarios_manually()
    {
        var engine = new ScenarioBuilder<Empty>()
            .UseDefaultEngine()
            .UseStandardScenarioSets()
            .Build();

        foreach (var setName in Sets.All)
        {
            var result = await engine
                .LoadSet<Empty>(setName)
                .Run(_ => Empty.Instance);

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
        var result = await new ScenarioBuilder<Empty>()
            .UseDefaultEngine()
            .UseStandardScenarioSets()
            .Build()
            .LoadSet<Empty>(name)
            .Run(_ => Empty.Instance);

        result.Assert();
    }
}
