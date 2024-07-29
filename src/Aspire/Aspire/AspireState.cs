namespace NeverTest.Aspire;

using global::Aspire.Hosting.Testing;
using NeverTests.AspireApp.Tests;


public class AspireState: IState
{
    public required IDistributedApplicationTestingBuilder Builder { get; init; }
    public required AspireHostOptions HostOptions { get; init; }
}
