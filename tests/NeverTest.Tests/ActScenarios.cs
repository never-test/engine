namespace NeverTest.Tests;

[TestClass]
public class ActScenarios : Runner
{
    [StandardSet(Sets.Acts.Basics)]
    public Task Basics(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Echo)]
    public Task Echo(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Folding)]
    public Task Folding(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Nesting)]
    public Task Naming(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Naming)]
    public Task Nesting(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Referencing)]
    public Task Referencing(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Acts.Advanced)]
    public Task Advanced(Scenario<State> s) => Run(s);

}
