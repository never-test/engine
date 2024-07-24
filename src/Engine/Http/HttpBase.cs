namespace NeverTest.Http;

using Logging;
internal abstract class HttpBase(IHttpClientFactory clientFactory) :
    IActStep<JToken, IState>
{
    protected Task<HttpResponseMessage> Send(
        HttpMethod method,
        HttpOptions options,
        HttpContent? content,
        IScenarioContext<IState> context)
    {
        context.Debug(options);

        var client = clientFactory.CreateClient(options.Name);
        var message = new HttpRequestMessage(method, options.Url)
        {
            Content = content
        };

        SetHeaders(options, message);

        return client.SendAsync(message);
    }

    private static void SetHeaders(HttpOptions options, HttpRequestMessage message)
    {
        foreach (var header in options.GetHeaders())
        {
            message.Headers.Add(header.Name, header.Values);
        }
    }

    public abstract Task<object?> Act(JToken input, IScenarioContext<IState> context);
}
