namespace NeverTest.Tests.Acting;

[TestClass]
public class Scenarios : Runner<MyScenarioState>
{
    [MySet("acting/1.basics.yaml")]
    public Task Basics(Scenario<MyScenarioState> s) => Run(s);
    
    [MySet("acting/2.echo.yaml")]
    public Task Echo(Scenario<MyScenarioState> s) => Run(s);
    
    [MySet("acting/3.folding.yaml")]
    public Task Folding(Scenario<MyScenarioState> s) => Run(s);

    [MySet("acting/4.naming.yaml")]
    public Task Naming(Scenario<MyScenarioState> s) => Run(s);
    
    [MySet("acting/5.nesting.yaml")]
    public Task Nesting(Scenario<MyScenarioState> s) => Run(s);
    
    [MySet("acting/6.referencing.yaml")]
    public Task Referencing(Scenario<MyScenarioState> s) => Run(s);
    
    [MySet("acting/666.advanced.yaml")]
    public Task Advanced(Scenario<MyScenarioState> s) => Run(s);

    protected override Task<MyScenarioState> CreateState() => Task.FromResult(new MyScenarioState(TestContext));
}