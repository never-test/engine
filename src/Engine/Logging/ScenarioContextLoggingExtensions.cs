namespace NeverTest.Logging;

public static class ScenarioContextLoggingExtensions
{
    public static void Info(this IScenarioContextBase context, string message, params object?[] args) => context.Write(LogLevel.Information, message, args);
    public static void Trace(this IScenarioContextBase context, string message, params object?[] args) => context.Write(LogLevel.Trace, message, args);
    public static void Dump(this IScenarioContextBase context, object? obj)
    {
        if (!context.Log.IsEnabled(LogLevel.Trace)) return;
        context.Write(LogLevel.Trace, "\u25cf dump: {object}", JsonConvert.SerializeObject(obj, context.JsonSerializerSettings));
    }
    public static void Debug(this IScenarioContextBase context, string message, params object?[] args) => context.Write(LogLevel.Debug, message, args);

    public static void Debug(this IScenarioContextBase context, object? obj)
    {
        if (!context.Log.IsEnabled(LogLevel.Debug)) return;
        context.Write(LogLevel.Debug, "\u25cb dump: {object}", JsonConvert.SerializeObject(obj, context.JsonSerializerSettings));
    }

    public static void Warn(this IScenarioContextBase context, string message, params object?[] args) => context.Write(LogLevel.Warning, message, args);
    private static void Write(this IScenarioContextBase context,
        LogLevel level,
        string message,
        params object?[] args)
    {
        if (!context.Log.IsEnabled(level)) return;

        var array = new object[args.Length + 1];
        array[0] = context.Indent;
        Array.Copy(args, 0, array, 1, args.Length);
        var symbol = level switch
        {
            LogLevel.Trace => '#',
            LogLevel.Debug => '*',
            LogLevel.Warning => '?',
            LogLevel.Error => '!',
            LogLevel.Critical => '@',
            _ => '-'
        };
        var msg = $"{symbol}{{Indent}} {message}";

#pragma warning disable CA2254
#pragma warning disable CA1848
        context.Log.Log(level, msg, array);
#pragma warning restore CA1848
#pragma warning restore CA2254
    }
}
