namespace NeverTest.Logging;

public static class ScenarioContextLoggingExtensions
{
    public static void Info(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Information, message, args);
    public static void Trace(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Trace, message, args);
    public static void Debug(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Debug, message, args);

    public static void Debug(this IScenarioContext context, object obj)
    {
        if (!context.Log.IsEnabled(LogLevel.Debug)) return;
        context.Write(LogLevel.Debug, "\u25cb dump: {object}", JsonConvert.SerializeObject(obj, context.JsonSerializerSettings));
    }

    public static void Warn(this IScenarioContext context, string message, params object?[] args) => context.Write(LogLevel.Warning, message, args);
    private static void Write(this IScenarioContext context,
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
            _ => ' '
        };
        var msg = $"{symbol}{{Indent}} {message}";

#pragma warning disable CA2254
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
        context.Log.Log(level, msg, array);
#pragma warning restore CA2254
    }
}
