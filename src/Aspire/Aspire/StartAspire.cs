namespace NeverTest.Aspire;

using global::Aspire.Hosting.ApplicationModel;
using global::Aspire.Hosting.Testing;

using Microsoft.Extensions.DependencyInjection;
using Logging;


public class StartAspire : IArrangeStep<AspireState>
{
    public async Task Arrange(IArrangeScenarioContext<AspireState> context)
    {
        var state = context.State();
        var app = await state.Builder.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        context.Services.AddSingleton(app);

        context.Trace("Starting distributed application");

        await app.StartAsync();


        if (state.HostOptions.Clients is not null)
        {
            foreach (var (resource, options) in state.HostOptions.Clients)
            {
                var opts = options ?? new();
                context.Trace("Creating '{resource}' client ", resource);
                context.Dump(options);

                await resourceNotificationService
                    .WaitForResourceAsync(resource, KnownResourceStates.Running)
                    .WaitAsync(opts.RunningWaitTimeout);

                var httpClient = app.CreateHttpClient(resource, opts.Endpoint);

                // register client with the same name in scenario's services
                // so http acts can pick it up
                context
                    .Services
                    .AddHttpClient(
                        resource,
                        client => client.BaseAddress = httpClient.BaseAddress
                    );
            }
        }
    }
}
