namespace NeverTest;

using Microsoft.Extensions.Logging;
using AspNet;

public static class BuilderExtensions
{
    public static ScenarioBuilder<AppState<TEntryPoint>> AddAspNet<TEntryPoint>(this ScenarioBuilder<AppState<TEntryPoint>> builder, LogLevel defaultLogLevel) where TEntryPoint : class =>
        builder
            .UseYaml()
            .UseDefaultEngine()
            .Verbosity(defaultLogLevel)
            .Arranges
            .Register(ArrangeSettings<TEntryPoint>.s_instance)
            .Always<StartHost<TEntryPoint>>()
            .Builder;
}
