using NeverTest.Acts;
using NeverTest.Asserts;
using NeverTest.MSTest;
using Newtonsoft.Json.Linq;

namespace NeverTest.Tests.Acts;

[TestClass]
public class Scenarios : Runner<MyCtx>
{

    [MySet("acts/echo.yaml")]
    public Task Echo(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acts/basics.yaml")]
    public Task Basics(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acts/folding.yaml")]
    public Task Folding(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acts/nesting.yaml")]
    public Task Nesting(Scenario<MyCtx> s) => Run(s);

    [MySet("acts/naming.yaml")]
    public Task Naming(Scenario<MyCtx> s) => Run(s);
    
    [MySet("acts/advanced.yaml")]
    public Task Advanced(Scenario<MyCtx> s) => Run(s);
    
    protected override Task<MyCtx> CreateState()
    {
        return Task.FromResult(new MyCtx(TestContext));
    }
}

public class MySet : ScenarioSetAttribute<MyCtx>
{
    public MySet(string set): base((set))
    {
        Builder
            .UsingYaml()
            .AddAct<Ping>("ping")
            .AddAct<Echo>("echo")
            .AddAct<Repeat, JObject, object?>("repeat")
            .AddAct<FooStep, JToken, object>("sdf")
            .AddAssert<Equals>("equals")
            ;
    }
}

public class MyCtx(TestContext context) : IState
{
    public TestContext Fooo { get; } = context;
    public Guid InstanceId { get; } = Guid.NewGuid();
}


public class FooStep : IActStep<JToken,object, MyCtx>
{
    public Task<object> Act(JToken? input, IScenarioContext<MyCtx> context)
    {
        

        return Task.FromResult<object>("Pong");
    }
}