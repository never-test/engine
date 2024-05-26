using Microsoft.Extensions.DependencyInjection;

namespace NeverTest;

using Acts;

public static class ScenarioBuilderExtensions
{
    public static ScenarioBuilder<TState> Verbosity<TState>(
        this ScenarioBuilder<TState> builder,
        LogLevel level) where TState : class, IState
    {
        builder.Services.AddLogging(x => x.SetMinimumLevel(level));
        return builder;
    }

    public static ScenarioBuilder<TState> UseDefaultEngine<TState>(this ScenarioBuilder<TState> builder) where TState : class, IState
    {
        return builder.Acts
            .Add(x=> x, o=>o.Name = "echo")
            .Add(_ => "pong", o=>o.Name = "ping")
            .Add((input, ctx) => ctx.State(), o=>o.Name = "state")

            .Register<Repeat, JObject>()
            .Builder;
    }
}
