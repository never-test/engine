namespace NeverTest.Arranges;

[AttributeUsage(AttributeTargets.Class)]
public class ArrangeAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public ArrangeOptions ToOptions() => new() {Name = Name};
}
