namespace NeverTest.Http;

using Acts;
using Logging;

[Act("post")]
internal class Post(IHttpClientFactory httpClientFactory) : IActStep<JToken, IState>
{
    public virtual async Task<object?> Act(
        JToken input,
        IScenarioContext<IState> context)
    {
        var options = HttpOptions.FromToken(
            input,
            context.JsonSerializer());

        var raw = options.Body?.ToString() ?? string.Empty;

        context.Trace(raw);

        var content = new StringContent(raw);

        return await SendPost(
            options,
            content,
            context);
    }

    protected async Task<HttpResponseMessage> SendPost(
        HttpOptions options,
        HttpContent content,
        IScenarioContext<IState> context)
    {

        context.Debug(options);


        var client = httpClientFactory.CreateClient(options.Name);
        var message = new HttpRequestMessage(HttpMethod.Post, options.Url);

        message.Content = content;

        foreach (var header  in options.GetHeaders())
        {
            message.Headers.Add(header.Name, header.Values);
        }

        var response = await client.SendAsync(message);
        return response;
    }
}
