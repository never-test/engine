namespace NeverTest.Tests;

[TestClass]
public class ActScenarios : Runner
{
    [StandardSet(Sets.Acts.Basics)]
    public Task Basics(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Echo)]
    public Task Echo(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Folding)]
    public Task Folding(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Nesting)]
    public Task Naming(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Naming)]
    public Task Nesting(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Referencing)]
    public Task Referencing(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Acts.Advanced)]
    public Task Advanced(Scenario<Empty> s) => Run(s);

}
