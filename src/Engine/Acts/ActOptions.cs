namespace NeverTest.Acts;

public class ActOptions
{
    public string? Name { get; set; }

    internal ActKey GetKey()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        return ActKey.FromString(Name);
    }
}
