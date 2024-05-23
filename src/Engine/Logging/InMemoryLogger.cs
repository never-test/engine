namespace NeverTest;

internal class InMemoryLogger(LogLevel minimumLogLevel) : ILogger
{
    private readonly List<(LogLevel, string, Exception?)> _messages = new();
    public IEnumerable<string> Logs => _messages.Select(m => m.Item2);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (logLevel >= minimumLogLevel)
        {
            _messages.Add((logLevel, formatter(state, exception), exception));
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;


    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}
