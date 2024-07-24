namespace NeverTest.Tests;

[TestClass]
[TestCategory("online")]
public class HttpScenarios : Runner
{
    [StandardSet(Sets.Http.Get)]
    public Task Get(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.Post)]
    public Task Post(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.Delete)]
    public Task Delete(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.Put)]
    public Task Put(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.Patch)]
    public Task Patch(Scenario<State> s) => Run(s);

    [StandardSet(Sets.Http.ResponseCodes)]
    public Task ResponseCodes(Scenario<State> s) => Run(s);
}
