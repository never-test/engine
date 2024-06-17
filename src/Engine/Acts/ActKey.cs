namespace NeverTest.Acts;

public record ActKey
{
    public string Value { get; private init; } = null!;
    public static ActKey FromString(string value) => new() {Value = value};

    public override string ToString() => Value;
}
