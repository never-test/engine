namespace NeverTest.Asserts;

public class AssertAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public AssertOptions ToOptions() => new() {Name = Name};
}
