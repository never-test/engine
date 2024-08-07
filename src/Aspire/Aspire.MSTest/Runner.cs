namespace NeverTest.Aspire;

using global::Aspire.Hosting.Testing;

using MSTest;
using NeverTests.AspireApp.Tests;
using Newtonsoft.Json.Linq;

public class Runner : RunnerBase<AspireState>
{
    protected override async Task<AspireState> CreateState(JToken? state)
    {
        ArgumentNullException.ThrowIfNull(state);

        var hostOptions = state.ToObject<AspireHostOptions>();
        ArgumentNullException.ThrowIfNull(hostOptions);

        var entryPoint = Type.GetType(hostOptions.EntryPoint);
        ArgumentNullException.ThrowIfNull(entryPoint);

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync(entryPoint, hostOptions.Args);

        var aspireState = new AspireState
        {
            HostOptions = hostOptions,
            Builder = appHost
        };

        return aspireState;
    }
}
