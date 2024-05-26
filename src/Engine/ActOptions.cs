namespace NeverTest;

public class ActOptions
{
    public string? Name { get; set; }

    internal ActKey GetKey()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        return ActKey.FromString(Name);
    }
}

public class ActAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public ActOptions ToOptions() => new() {Name = Name};
}
