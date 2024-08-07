namespace NeverTest.AspNet;

internal sealed class DelegatingFactoryOptions
{
    public required Func<HttpClient> TestFactory { get; init; }
    public required string TestClientName { get; init; }
}
