namespace NeverTest.Asserts;

public record AssertKey(string Value)
{
    public static AssertKey FromString(string name) => new(name);
}
