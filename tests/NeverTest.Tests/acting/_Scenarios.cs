namespace NeverTest.Tests.Acting;

[TestClass]
public class Scenarios : Runner<MyCtx>
{
    [MySet("acting/1.basics.yaml")]
    public Task Basics(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acting/2.echo.yaml")]
    public Task Echo(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acting/3.folding.yaml")]
    public Task Folding(Scenario<MyCtx> s) => Run(s);

    [MySet("acting/4.naming.yaml")]
    public Task Naming(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acting/5.nesting.yaml")]
    public Task Nesting(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acting/6.referencing.yaml")]
    public Task Refrencing(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acting/666.advanced.yaml")]
    public Task Advanced(Scenario<MyCtx> s) => Run(s);

    protected override Task<MyCtx> CreateState() => Task.FromResult(new MyCtx(TestContext));
}