namespace NeverTests.AspireApp.Tests;

public class AspireHostOptions
{
    public required string EntryPoint { get; init; }
    public string[] Args { get; init; } = [];
    public Dictionary<string, AspireClientOptions>? Clients { get; init; }
}
