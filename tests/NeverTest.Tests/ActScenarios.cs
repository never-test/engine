namespace NeverTest.Tests;

using NeverTest.Scenarios.Yaml;

[TestClass]
public class Scenarios : Runner
{
    [StandardSet(Sets.Basics)]
    public Task Basics(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Echo)]
    public Task Echo(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Folding)]
    public Task Folding(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Nesting)]
    public Task Naming(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Naming)]
    public Task Nesting(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Referencing)]
    public Task Referencing(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Advanced)]
    public Task Advanced(Scenario<State> s) => Run(s);

}
