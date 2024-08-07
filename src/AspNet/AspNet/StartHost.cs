namespace NeverTest.AspNet;

using Microsoft.Extensions.DependencyInjection;

public class StartHost<TEntryPoint> : IArrangeStep<AppState<TEntryPoint>> where TEntryPoint : class
{
    public Task Arrange(IArrangeScenarioContext<AppState<TEntryPoint>> context)
    {
        var state = context.State();

        context
            .Services
            .Decorate<IHttpClientFactory, DelegatingHttpClientFactory>()
            .AddSingleton(new DelegatingFactoryOptions
            {
                TestFactory = ClientFactory,
                TestClientName = state.Options.Client
            });

        return Task.CompletedTask;

        HttpClient ClientFactory() => state.Factory.CreateClient();
    }
}
