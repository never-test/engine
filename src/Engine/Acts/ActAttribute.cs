namespace NeverTest.Acts;

[AttributeUsage(AttributeTargets.Class)]
public class ActAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public ActOptions ToOptions() => new() {Name = Name};
}
