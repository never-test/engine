namespace NeverTest.Acts;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class ActBuilder<TState>(ScenarioBuilder<TState> builder) where TState : IState
{
    private readonly Dictionary<ActKey, ActInstance> _acts = new();
    internal IReadOnlyDictionary<ActKey, ActInstance> Instances => _acts;
    public ScenarioBuilder<TState> Builder => builder;
    public ActBuilder<TState> Register<TAct>(Action<ActOptions>? configure = null) where TAct : class, IActStep<JToken, TState> => Register<TAct, JToken>(configure);
    public ActBuilder<TState> Register<TAct, TInput>(Action<ActOptions>? configure = null) where TAct : class, IActStep<TInput, TState> where TInput : class
    {
        var options = CreateOptions<TAct, TInput>(configure);
        var key = options.GetKey();

        builder.Services.TryAddTransient<TAct>();
        builder.Services.TryAddTransient<ActRegistration<TAct, TInput>>(sp =>new()
        {
            Key = key,
            Act = sp.GetRequiredService<TAct>
        });

        RegisterStep<TAct, TInput>(options);

        return this;
    }

    public ActBuilder<TState> Add(
        Func<JToken?, object?> callback,
        Action<ActOptions> configure) => Add<JToken?>((input, _) => callback(input), configure);

    public ActBuilder<TState> Add(
        Func<JToken?, IScenarioContext<TState>, object?> callback,
        Action<ActOptions> configure) => Add<JToken?>(callback, configure);
    public ActBuilder<TState> Add<TInput>(
        Func<TInput, IScenarioContext<TState>, object?> callback,
        Action<ActOptions> configure) where TInput : class?
    {
        var options = CreateOptions<CallbackAct<TInput, TState>, TInput>(configure);
        var key = options.GetKey();

        builder.Services.AddTransient<ActRegistration<CallbackAct<TInput, TState>, TInput>>(sp =>new()
        {
            Key = key,
            Act = ()=> new CallbackAct<TInput, TState>(callback)
        });

        RegisterStep<CallbackAct<TInput, TState>, TInput>(options);

        return this;
    }

    private static ActOptions CreateOptions<TAct, TInput>(Action<ActOptions>? configure)
        where TAct : class, IActStep<TInput, TState> where TInput : class?
    {
        var actType = typeof(TAct);
        var options = typeof(TAct)
            .GetCustomAttribute<ActAttribute>()?
            .ToOptions() ?? new()
        {
            Name = actType.Name.ToLower()
        };

        configure?.Invoke(options);
        return options;
    }

    private class ActRegistration<TAct, TInput>
        where TAct : IActStep<TInput, TState>
    {
        public required ActKey Key { get; init; }
        public required Func<TAct> Act { get; init; }
    }

    /// <summary>
    /// Expects TAct to be registered as key service with the ActKey
    /// </summary>
    /// <param name="options"></param>
    /// <typeparam name="TAct"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    private void RegisterStep<TAct, TInput>(ActOptions options) where TAct : class, IActStep<TInput, TState> where TInput : class?
    {
        var key = options.GetKey();
        if (_acts.ContainsKey(key))
        {
            throw new ArgumentException($"Act with the '{key.Value}' name has been already registered.");
        }

        var step = new ActInstance
        {
            Invocation = async (input, sc) =>
            {
                var registration = sc
                    .Engine
                    .Provider
                    .GetServices<ActRegistration<TAct, TInput>>()
                    .FirstOrDefault(x => x.Key == key)
                    ?? throw new ArgumentException($"Registration for '{key}' was not found.");

                return await registration
                    .Act()
                    .Act(input != null ? Convert<TInput>(input) : default!, (IScenarioContext<TState>)sc);
            },
            StepType = typeof(TAct),
        };

        _acts.Add(key, step);
    }

    private static TInput Convert<TInput>(JToken input) where TInput : class?
    {
        if (typeof(TInput) == typeof(JObject) && input is JObject jObject)
        {
            return (jObject as TInput)!;
        }

        return input switch
        {
            JValue sv when typeof(TInput) == typeof(string) => (TInput)sv.Value!,
            JValue jv when typeof(TInput).IsPrimitive => (TInput)jv.Value!,

            _ => input.ToObject<TInput>()!
        };
    }
}
