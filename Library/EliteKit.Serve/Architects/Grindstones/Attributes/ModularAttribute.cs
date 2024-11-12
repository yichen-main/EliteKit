namespace EliteKit.Serve.Architects.Grindstones.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ModularAttribute(string name = nameof(AsyncCallback)) : Attribute
{
    public string Name { get; init; } = name;
}