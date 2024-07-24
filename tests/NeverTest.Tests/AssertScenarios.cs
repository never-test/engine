namespace NeverTest.Tests;

[TestClass]
public class AssertScenarios : Runner
{
    [StandardSet(Sets.Asserts.Exceptions)]
    public Task Exceptions(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Asserts.Basics)]
    public Task Basics(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Asserts.Referencing)]
    public Task Referencing(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Asserts.Equals)]
    public Task Equals(Scenario<State> s) => Run(s);
}
