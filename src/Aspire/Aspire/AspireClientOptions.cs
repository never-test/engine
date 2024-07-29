namespace NeverTests.AspireApp.Tests;

public class AspireClientOptions
{
    public string? Endpoint { get; init; }
    public TimeSpan RunningWaitTimeout { get; init; }  = TimeSpan.FromSeconds(5);
}
