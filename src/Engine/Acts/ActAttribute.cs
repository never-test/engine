namespace NeverTest.Acts;

public class ActAttribute(string name) : Attribute
{
    private string Name { get; } = name;
    public ActOptions ToOptions() => new() {Name = Name};
}
