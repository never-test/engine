namespace NeverTest.AspNet;

internal sealed class DelegatingHttpClientFactory(
    IHttpClientFactory defaultFactory,
    DelegatingFactoryOptions options)
    : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => options
        .TestClientName.Equals(name, StringComparison.Ordinal)
        ? options.TestFactory()
        : defaultFactory.CreateClient(name);
}
