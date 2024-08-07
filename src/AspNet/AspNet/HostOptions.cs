namespace NeverTest.AspNet;

public class HostOptions
{
    public IReadOnlyDictionary<string, string> Settings { get; init; } = new Dictionary<string, string>();
    public string Client { get; init; } = string.Empty;
}
