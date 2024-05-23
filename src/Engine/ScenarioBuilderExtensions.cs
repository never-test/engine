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
            .Register<Ping>("ping")
            .Register<Echo>("echo")
            .Register<Repeat, JObject>("repeat")
            .Builder;
    }
}
