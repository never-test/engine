namespace NeverTest.Arranges;

public class ArrangeOptions
{
    public string? Name { get; set; }

    internal ArrangeKey GetKey()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        return ArrangeKey.FromString(Name);
    }
}
