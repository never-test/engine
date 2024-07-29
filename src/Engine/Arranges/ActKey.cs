namespace NeverTest.Arranges;

public record ArrangeKey
{
    public string Value { get; private init; } = null!;
    public static ArrangeKey FromString(string value) => new() {Value = value};

    public override string ToString() => Value;
}
