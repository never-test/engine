using NeverTest.Logging;
using Newtonsoft.Json;

namespace NeverTest.Asserts;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class AssertBuilder<TState>(ScenarioBuilder<TState> builder) where TState : IState
{
    //todo: can this be generalized to be used in acts?

    private readonly Dictionary<AssertKey, AssertInstance> _asserts = new();
    public ScenarioBuilder<TState> Builder => builder;
    internal IReadOnlyDictionary<AssertKey, AssertInstance> Instances => _asserts;

    public AssertBuilder<TState> Register<TAssert>(Action<AssertOptions>? configure = null) where TAssert :
        class, IAssertStep<TState>
    {
        var options = CreateOptions<TAssert>(configure);
        var key = options.GetKey();

        builder.Services.TryAddTransient<TAssert>();
        builder.Services.TryAddTransient<AssertRegistration<TAssert>>(sp=> new AssertRegistration<TAssert>
        {
            Key = key,
            Instance = sp.GetRequiredService<TAssert>
        });

        RegisterStep<TAssert>(options);
        return this;
    }

    public AssertBuilder<TState> Add(
        Action<JToken?,JToken?, IScenarioContext<TState>> callback,
        Action<AssertOptions> configure)
    {
        var options = CreateOptions<CallbackAssert<TState>>(configure);
        var key = options.GetKey();

        builder.Services.AddTransient<AssertRegistration<CallbackAssert<TState>>>(sp =>new()
        {
            Key = key,
            Instance = ()=> new CallbackAssert<TState>(callback)
        });

        RegisterStep<CallbackAssert<TState>>(options);

        return this;
    }

    private void RegisterStep<TAssert>(AssertOptions options) where TAssert : class, IAssertStep<TState>
    {
        var key = options.GetKey();
        if (_asserts.ContainsKey(key))
        {
            throw new ArgumentException($"Act with the '{key.Value}' name has been already registered.");
        }

        var step = new AssertInstance
        {
            Invocation = async (actual,expected, sc) =>
            {
                sc.Info("{Path}", expected?.Path ?? key.Value);
                sc.Debug("\u21e8 act: {Actual}", actual?.ToString(Formatting.None));
                sc.Debug("\u21e8 exp: {Expected}", expected?.ToString(Formatting.None));

                var registration = sc
                                       .Engine
                                       .Provider
                                       .GetServices<AssertRegistration<TAssert>>()
                                       .FirstOrDefault(x => x.Key == key)
                                   ?? throw new ArgumentException($"Registration for '{key}' was not found.");

                await registration
                    .Instance()
                    .Assert(actual, expected, (IScenarioContext<TState>)sc);
            },
            StepType = typeof(TAssert)
        };

        _asserts.Add(key, step);
    }

    private class AssertRegistration<TAssert>
    {
        public required AssertKey Key { get; init; }
        public required  Func<TAssert> Instance { get; init; }
    }

    private static AssertOptions CreateOptions<TAssert>(Action<AssertOptions>? configure)
    {
        var assertType = typeof(TAssert);
        var options = assertType
            .GetCustomAttribute<AssertAttribute>()?
            .ToOptions() ?? new()
        {
            Name = assertType.Name.ToLower()
        };

        configure?.Invoke(options);
        return options;
    }
}
