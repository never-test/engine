namespace NeverTest;

using Newtonsoft.Json;

public static class ExceptionExtensions
{
    private static readonly JsonSerializerSettings s_defaultSettings = new();

    public static JObject ToJson(
        this Exception ex,
        JsonSerializerSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(ex);

        var info = new ExceptionInfo(ex);
        var serializer = JsonSerializer.Create(settings ?? s_defaultSettings);

        return JObject.FromObject(info, serializer);
    }
}
internal class ExceptionInfo
{
    internal ExceptionInfo(Exception exception)
    {
        Type = exception.GetType().FullName;
        Message = exception.Message;
        Source = exception.Source;
        StackTrace = exception.StackTrace;
        if (exception.InnerException is not null)
        {
            InnerException = new ExceptionInfo(exception.InnerException);
        }
    }

    public string? Type { get; set; }
    public string Message { get; set; }
    public string? Source { get; set; }
    public string? StackTrace { get; set; }
    public ExceptionInfo? InnerException { get; set; }
}
