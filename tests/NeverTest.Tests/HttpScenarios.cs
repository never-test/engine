namespace NeverTest.Tests;

[TestClass]
[TestCategory("online")]
public class HttpScenarios : Runner
{
    [StandardSet(Sets.Http.Get)]
    public Task Get(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.Post)]
    public Task Post(Scenario<State> s) => Run(s);
}
