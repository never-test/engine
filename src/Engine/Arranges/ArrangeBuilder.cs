using System.Globalization;

namespace NeverTest.Arranges;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public class ArrangeBuilder<TState>(ScenarioBuilder<TState> builder) where TState : IState
{
    private readonly Dictionary<ArrangeKey, ArrangeInstance> _arranges = new();
    internal IReadOnlyDictionary<ArrangeKey, ArrangeInstance> Instances => _arranges;
    public ScenarioBuilder<TState> Builder => builder;

    /// <summary>
    /// Registers arrange that always runs.
    /// </summary>
    /// <typeparam name="TArrange"></typeparam>
    /// <returns></returns>
    public ArrangeBuilder<TState> Always<TArrange>() where TArrange : class, IArrangeStep<TState>
    {
        Builder.Services.AddTransient<IArrangeStep<TState>, TArrange>();
        return this;
    }
    public ArrangeBuilder<TState> Register<TInput>(
        IArrangeStep<TInput, TState> instance,
        Action<ArrangeOptions>? configure = null) where TInput : class
    {
        ArgumentNullException.ThrowIfNull(instance);
        var options = CreateOptions(instance.GetType(), configure);

        RegisterStep(instance, options);

        return this;
    }

    public ArrangeBuilder<TState> Add(
        Func<JToken?, object?> callback,
        Action<ArrangeOptions> configure) => Add<JToken?>((input, _) => callback(input), configure);

    public ArrangeBuilder<TState> Add(
        Action<JToken?, IArrangeScenarioContext<TState>> callback,
        Action<ArrangeOptions> configure) => Add<JToken?>(callback, configure);
    public ArrangeBuilder<TState> Add<TInput>(
        Action<TInput, IArrangeScenarioContext<TState>> callback,
        Action<ArrangeOptions> configure) where TInput : class?
    {
        var instance = new CallbackArrange<TInput, TState>(callback);
        var options = CreateOptions(instance.GetType(), configure);

        RegisterStep(instance, options);

        return this;
    }

    private static ArrangeOptions CreateOptions(Type arrangeStepType, Action<ArrangeOptions>? configure)
    {
        var options = arrangeStepType
            .GetCustomAttribute<ArrangeAttribute>()?
            .ToOptions() ?? new()
        {
            Name = arrangeStepType.Name.ToLower(CultureInfo.InvariantCulture)
        };

        configure?.Invoke(options);
        return options;
    }

    private class ArrangeRegistration<TArrange, TInput>
        where TArrange : IArrangeStep<TInput, TState>
    {
        public required ArrangeKey Key { get; init; }
        public required Func<TArrange> Arrange { get; init; }
    }



    private void RegisterStep<TInput>(IArrangeStep<TInput, TState> instance, ArrangeOptions options) where TInput : class?
    {
        var key = options.GetKey();
        if (_arranges.ContainsKey(key))
        {
            throw new ArgumentException($"Act with the '{key.Value}' name has been already registered.");
        }

        var step = new ArrangeInstance
        {
            Invocation = async (input, sc) =>
            {
                await instance.Arrange(input != null ? Convert<TInput>(input) : default!, (IArrangeScenarioContext<TState>)sc);

            },
            StepType = instance.GetType(),
        };

        _arranges.Add(key, step);
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
