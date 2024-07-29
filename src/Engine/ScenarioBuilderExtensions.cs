namespace NeverTest;

using FluentAssertions.Json;
using Microsoft.Extensions.DependencyInjection;

using Asserts;
using Acts;
using Http;

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
        return builder
            .Acts
                .Add(x=> x, o=>o.Name = "echo")
                .Add(_ => "pong", o=>o.Name = "ping")
                .Add((input, ctx) => ctx.State(), o=>o.Name = "state")
                .Register<Repeat, JObject>()
            .Builder
            .Asserts
                .Add((actual,e, _)=> actual.Should().BeNull(), o=>o.Name = "nil")
                .Add((actual,e, _)=> actual.Should().NotBeNull(), o=>o.Name = "exists")
                .Register<Equals>()
                .Register<Matches>()
                .Register<Select>()
                .Register<Contains>()
                .Register<HasCount>()
            .Builder
                .UseHttp();
    }

}
