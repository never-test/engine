namespace NeverTest.Building;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

public class ActBuilder<TState>(ScenarioBuilder<TState> builder) where TState : IState
{
    private readonly Dictionary<ActKey, StepInstance> _acts = new();
    internal IReadOnlyDictionary<ActKey, StepInstance> Instances => _acts;
    public ScenarioBuilder<TState> Builder => builder;

    public ActBuilder<TState> Register<TAct>(string key) where TAct : class, IActStep<JToken, TState>
        => Register<TAct, JToken>(key);


    public ActBuilder<TState> Register<TAct, TInput>(string key) where TAct : class, IActStep<TInput, TState> where TInput : class
    {
        builder.Services.AddTransient<TAct>();

        var step = new StepInstance
        {
            Invocation = async (input, sc) => await sc
                .Engine
                .Provider
                .GetRequiredService<TAct>()
                .Act(input != null ? Convert<TInput>(input) : default!, (IScenarioContext<TState>)sc),
            StepType = typeof(TAct),
        };

        _acts.Add(new(key), step);
        return this;
    }

    private static TInput Convert<TInput>(JToken input) where TInput : class
    {
        if (typeof(TInput) == typeof(JObject) && input is JObject jObject)
        {
            return (jObject as TInput)!;
        }

        return input.ToObject<TInput>()!;
    }
}
