namespace NeverTest.Asserts;

[AttributeUsage(AttributeTargets.Class)]
public class AssertAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public AssertOptions ToOptions() => new() {Name = Name};
}
