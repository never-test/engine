namespace NeverTest.Asserts;

public class AssertOptions
{
    public string? Name { get; set; }

    internal AssertKey GetKey()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);

        return AssertKey.FromString(Name);
    }
}

