namespace NeverTest.Tests;

[TestClass]
[TestCategory("online")]
public class HttpScenarios : DefaultRunner
{
    [StandardSet(Sets.Http.Get)]
    public Task Get(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Http.Post)]
    public Task Post(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Http.Delete)]
    public Task Delete(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Http.Put)]
    public Task Put(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Http.Patch)]
    public Task Patch(Scenario<Empty> s) => Run(s);

    [StandardSet(Sets.Http.ResponseCodes)]
    public Task ResponseCodes(Scenario<Empty> s) => Run(s);
}
