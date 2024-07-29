namespace NeverTest.Tests;

[TestClass]
public class AssertScenarios : Runner
{
    [StandardSet(Sets.Asserts.Exceptions)]
    public Task Exceptions(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Asserts.Basics)]
    public Task Basics(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Asserts.Referencing)]
    public Task Referencing(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Asserts.EqualsAssert)]
    public Task Equals(Scenario<Empty> s) => Run(s);
}
